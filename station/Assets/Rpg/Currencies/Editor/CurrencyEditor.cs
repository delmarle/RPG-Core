using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Station.Editor
{
    public static class CurrencyEditor
    {
        private const string EDITOR_CURRENCY_PATH = "Assets/Content/Currencies/";
        private static CurrenciesDb _currenciesDb;
        private static int _selectedEntryIndex = 0;
        private static Vector2 _propertyScrollPos;
        private static Vector2 _viewScrollPos;
        private static Vector2 _scrollPos;
        private static string _newCurrencyName;
        private static bool CacheDbs()
        {
            bool missing = false;
            if (_currenciesDb == null)
            {
                _currenciesDb = (CurrenciesDb) EditorStatic.GetDb(typeof(CurrenciesDb));
                missing = true;
            }

            return missing;
        }

        public static void Draw()
        {
            CacheDbs();
            EditorGUI.BeginChangeCheck();
            {
                GUILayout.BeginHorizontal();
                DrawRaceList();
                ListView(_selectedEntryIndex);
                GUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                _currenciesDb.ForceRefresh();
            }
        }

        private static void DrawRaceList()
        {
            _selectedEntryIndex = DrawGenericSelectionList(_selectedEntryIndex, _propertyScrollPos, out _propertyScrollPos, "user");
        }
        
        private static int DrawGenericSelectionList(int selectedIndex, Vector2 currentScrollPos, out Vector2 propertyScrollPos, string iconName)
        {
            var entries = _currenciesDb.ListEntryNames();
            GUILayout.BeginVertical("box", GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH), GUILayout.ExpandHeight(true));
            {
                propertyScrollPos = GUILayout.BeginScrollView(currentScrollPos, GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true));
                {
                    GUILayout.Label("SELECT " + _currenciesDb.ObjectName() + ": ",
                        GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH - 30));
                    var toolbarOptions = new GUIContent[_currenciesDb.Count()];
                    for (int i = 0; i < _currenciesDb.Count(); i++)
                    {
                        IStationIcon foundIcon = _currenciesDb.GetEntry(i) as IStationIcon;
                        Texture2D icon = null;
                        if (foundIcon != null)
                        {
                            if (foundIcon.GetIcon() != null)
                            {
                                Object txture = foundIcon?.GetIcon()?.texture;
                                if (txture)
                                {
                                    string nameIcon = AssetDatabase.GetAssetPath(txture);
                                    icon = EditorGUIUtility.FindTexture(nameIcon);
                                }
                            }
                        }

                        if (icon == null)
                        {
                            icon = EditorStatic
                                .GetEditorTexture(iconName); //intern ? EditorGUIUtility.FindTexture(iconName) :
                        }

                        toolbarOptions[i] = new GUIContent(entries[i], icon, "");
                    }

                    var previousIndex = selectedIndex;

                    float lElemH2 = toolbarOptions.Length * 40;
                    selectedIndex = GUILayout.SelectionGrid(selectedIndex, toolbarOptions, 1,
                        EditorStatic.VerticalBarStyle, GUILayout.Height(lElemH2),
                        GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH - 10));
                    if (previousIndex != selectedIndex) EditorStatic.ResetFocus();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            return selectedIndex;
        }
        
        private static void ListView(int selectedCurrency)
        {
          
            GUILayout.BeginVertical();
            DrawHeader();
    
        

           
            GUILayout.BeginHorizontal("box");
        
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            {
                //RacePanel(race,selectedRace);
                DrawCurrency();
            }
            EditorGUILayout.EndScrollView();
            
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }


        private static void DrawHeader()
        {
            GUILayout.BeginVertical();
            EditorStatic.DrawSectionTitle(32,"Add a new currency: ");
            _newCurrencyName = EditorGUILayout.TextField("new currency name: ", _newCurrencyName);
            
            #region Add Button

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                string secondLine = "\n Total " + _currenciesDb.ObjectName() + " " + +_currenciesDb.Count();
                if (EditorStatic.ButtonPressed("Add " + _currenciesDb.ObjectName() + secondLine, Color.white, "plus"))
                {
                    if (string.IsNullOrEmpty(_newCurrencyName))
                    {
                        return;
                    }
                    
                    var so = ScriptableHelper.CreateScriptableObject<CurrencyModel>(EDITOR_CURRENCY_PATH, "currency_"+_newCurrencyName + ".asset");
                    _newCurrencyName = String.Empty;
                    AssetDatabase.SaveAssets();
                        
                    _currenciesDb.Add(so);
                    _selectedEntryIndex = _currenciesDb.Count() - 1;
                    _currenciesDb.ForceRefresh();
                    EditorStatic.ResetFocus();
                }
            }
            GUILayout.EndHorizontal();
            #endregion
            GUILayout.EndVertical();
        }

        private static void DrawCurrency()
        {
            var currencyCount = _currenciesDb.Count();
            if (_selectedEntryIndex == -1) return;
            if (currencyCount == 0) return;
            if (currencyCount < _selectedEntryIndex + 1) _selectedEntryIndex = 0;
            var currency = _currenciesDb.GetEntry(_selectedEntryIndex);
            if (currency == null)
            {
                _currenciesDb.Remove(currency);
                _currenciesDb.ForceRefresh();
                return;
            }
            EditorStatic.DrawSectionTitle(currency.name,21);
            EditorGUILayout.BeginHorizontal();
            EditorStatic.DrawLocalization(currency.Name, "base currency name:"); 
            currency.Icon = (Sprite)EditorGUILayout.ObjectField("Icon", currency.Icon, typeof(Sprite), false);
            EditorGUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Description",GUILayout.Width(70));
                EditorStatic.DrawLocalizationLabel(currency.Description, 45);
            }
            GUILayout.EndHorizontal();
            EditorStatic.DrawLargeLine();
            if (EditorStatic.ButtonPressed("Add sub currency" , Color.white, "plus"))
            {
                currency.SubValues.Add(new CurrencySubValue());
            }
            EditorStatic.DrawLargeLine();
            foreach (var subValue in  currency.SubValues)
            {
                GUILayout.BeginHorizontal();
                EditorStatic.DrawLocalization(subValue.Name, "name:"); 
                subValue.PreviousEquivalent = EditorGUILayout.IntField("equivalent from previous:", subValue.PreviousEquivalent);
                GUILayout.EndHorizontal();
                subValue.Icon = (Sprite)EditorGUILayout.ObjectField("Icon:", subValue.Icon, typeof(Sprite), false);
            }
            EditorStatic.DrawThinLine();
        }
    }
}