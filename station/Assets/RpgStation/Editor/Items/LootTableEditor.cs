using System.Collections;
using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static partial class LootTableEditor
    {
        private static ItemsDb _itemsDb;
        private static LootTableDb _lootTableDb;

        private static int _selectedEntryIndex = 0;
        private static Vector2 _propertyScrollPos;
        private static Vector2 _viewScrollPos;
        private static Vector2 _scrollPos;

        
        private static bool CacheDbs()
        {
            bool missing = false;

            if (_lootTableDb == null)
            {
                _lootTableDb = (LootTableDb) EditorStatic.GetDb(typeof(LootTableDb));
                missing = true;
            }

            if (_itemsDb == null)
            {
                _itemsDb = (ItemsDb) EditorStatic.GetDb(typeof(ItemsDb));
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
                _lootTableDb.ForceRefresh(); 
            }
        }

        #region [[DRAW EXTERNAL TABLE REFERENCE]]

        public static string DrawExternalTableReference(string tableId)
        {
            CacheDbs();
            if (string.IsNullOrEmpty(tableId))
            {
                GUILayout.BeginHorizontal();
                if (_lootTableDb.Count() > 0)
                {
                    if (EditorStatic.SizeableButton(120, 60, "use existing table", ""))
                    {
                        tableId = _lootTableDb.GetKey(0);
                    } 
                }
                
                if (EditorStatic.SizeableButton(120, 60, "create new table", ""))
                {
                    _lootTableDb.Add(new LootTableModel());
                    var lastIndex = _lootTableDb.Count() - 1;
                    tableId = _lootTableDb.GetKey(lastIndex);
                    _lootTableDb.ForceRefresh(); 
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                //draw assigned version
                var entriesNames = _lootTableDb.ListEntryNames();
                int currentIndex = 0;
                if (_lootTableDb.HasKey(tableId))
                {
                    currentIndex = _lootTableDb.GetIndex(tableId);
                }
                 
                currentIndex = EditorGUILayout.Popup(currentIndex, entriesNames);
                tableId = _lootTableDb.GetKey(currentIndex);
                if (EditorStatic.SizeableButton(120, 60, "disable", ""))
                {
                    return string.Empty;
                }

                GUILayout.EndHorizontal();
            }
            
          
            return tableId;
        }

        #endregion
        
            #region [[ DRAW DB VERSION]]

    private static void DrawRaceList()
    {
      _selectedEntryIndex = EditorStatic.DrawGenericSelectionList(_lootTableDb, _selectedEntryIndex, _propertyScrollPos,out _propertyScrollPos,"user",false);
    }
    private static void ListView(int selectedLootTable)
    {
      var lootTablesCount = _lootTableDb.Count();
      if (selectedLootTable == -1) return;
      if (lootTablesCount == 0) return;
      if (lootTablesCount < selectedLootTable+1) selectedLootTable = 0;
      
      var race = _lootTableDb.GetEntry(selectedLootTable);
      GUILayout.BeginHorizontal("box");
      {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
        {
          LootTablePanel(race,selectedLootTable);
        }
        EditorGUILayout.EndScrollView();
      }
      GUILayout.EndHorizontal();
    }

    private static void LootTablePanel(LootTableModel lootStaticData,int selectedRace)
    {
      GUILayout.Label("EDIT RACE:",GUILayout.Width(70));
      EditorStatic.DrawLargeLine(5);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      { 
          GUILayout.BeginVertical();
        {
          GUILayout.Space(5);
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Name",GUILayout.Width(70));
            lootStaticData.Description = GUILayout.TextField(lootStaticData.Description);
          }
          GUILayout.EndHorizontal();
          GUILayout.Space(3);
        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete loot Table?",
            "Do you want to delete: "+lootStaticData.Description,"Delete","Cancel"))
          {
              _lootTableDb.Remove(lootStaticData);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
      EditorStatic.DrawThinLine(10);
      DrawLootTable(lootStaticData);
    }
    
    #endregion
        
        #region DRAWERS
        public static LootTableModel DrawLootTable(LootTableModel model)
        {
            CacheDbs();
            
            EditorStatic.DrawSectionTitle("Loots", 0);
            if (EditorStatic.SizeableButton(200, 32,"Add One", "plus"))
            {
                model.Loots.Add(new LootModel());
            }
            EditorStatic.DrawLargeLine(5);
            if (_itemsDb.Count() == 0)
            {
                EditorGUILayout.HelpBox("there is no items in the items db", MessageType.Info);
            }
            else
            {
                foreach (var entryLoot in model.Loots)
                {
                    EditorGUILayout.BeginHorizontal("box");
                    var foundItem = entryLoot.ItemId != null? _itemsDb.GetEntry(entryLoot.ItemId): null;
                    if (foundItem == null)
                    {
                        entryLoot.ItemId = _itemsDb.GetKey(0);
                    }

                    int indexItem = _itemsDb.GetIndex(entryLoot.ItemId);

                    indexItem = EditorGUILayout.Popup(indexItem, _itemsDb.ListEntryNames());

                    entryLoot.ItemId = _itemsDb.GetKey(indexItem);

                    entryLoot.Chance = EditorGUILayout.Slider("Drop rate (%): ", entryLoot.Chance, 0, 100);
                    entryLoot.QuantityMin = EditorGUILayout.IntField("Amount min - max: ", entryLoot.QuantityMin);
                    entryLoot.QuantityMax = EditorGUILayout.IntField(entryLoot.QuantityMax, GUILayout.Width(60));
                    if (EditorStatic.SizeableButton(100, 18, "delete", "cross"))
                    {
                        model.Loots.Remove(entryLoot);
                        GUIUtility.ExitGUI();
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            
            return model;
        }
        
        #endregion
    }
}