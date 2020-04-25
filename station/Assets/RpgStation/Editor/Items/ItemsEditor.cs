using System.Collections.Generic;
using System.Linq;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static partial class ItemsEditor
    {
        #region FIELDS

        
        private static List<string> _itemsFilterTags = new List<string>();
        private static int _selectedItemEntry;
        private static Vector2 _itemScrollPos;
        private static Texture2D _selectedTexture;
        private static Vector2 _itemListPropertyScrollPos;
        private static bool _pickingTexture;
        #endregion
        private static void DrawItems()
        {
            if (_itemsDb == null)
            {
                EditorGUILayout.HelpBox("Items Db is missing", MessageType.Error);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                DrawItemsList();
                DrawItemView();
                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawItemsTagFilter()
        {
            //show list horizontal
            //show popup
        }
        
        private static void DrawItemsList()
        {
            _selectedItemEntry = EditorStatic.DrawGenericSelectionList(_itemsDb, _selectedItemEntry, _itemListPropertyScrollPos,out _itemListPropertyScrollPos,"user",false);
        }
        
        private static void DrawItemView()
        {
            var raceCount = _itemsDb.Count();
            if (_selectedItemEntry == -1) return;
            if (raceCount == 0) return;
            if (raceCount < _selectedItemEntry+1) _selectedItemEntry = 0;
      
            var item = _itemsDb.GetEntry(_selectedItemEntry);
            GUILayout.BeginHorizontal("box");
            {
                _itemScrollPos = EditorGUILayout.BeginScrollView(_itemScrollPos, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
                {
                    ItemPanel(item,_selectedItemEntry);
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();
        }
        
         private static void ItemPanel(ItemModel itemModel,int selectedRace)
    {
      GUILayout.Label("EDIT Item:",GUILayout.Width(70));
      EditorStatic.DrawLargeLine(5);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
        if (itemModel.Icon)
          _selectedTexture = itemModel.Icon.texture;
        else
          _selectedTexture = null;
        if (GUILayout.Button(_selectedTexture, GUILayout.Width(EditorStatic.EDITOR_BUTTON_SIZE),GUILayout.Height(EditorStatic.EDITOR_BUTTON_SIZE)))
        {
          int controllerId = GUIUtility.GetControlID(FocusType.Passive);
          EditorGUIUtility.ShowObjectPicker<Sprite>(null,false,null,controllerId);
          _pickingTexture = true;
        }
        string commandName = Event.current.commandName;
        if (_pickingTexture && commandName == EditorStatic.EDITOR_OBJECT_PICKER_COMMAND_NAME)
        {
            _itemsDb.GetEntry(selectedRace).Icon = EditorGUIUtility.GetObjectPickerObject() as Sprite;
          _pickingTexture = false;
        }
        
        GUILayout.BeginVertical();
        {
          GUILayout.Space(5);
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Name",GUILayout.Width(70));
            itemModel.Name.Key = GUILayout.TextField(itemModel.Name.Key);
          }
          GUILayout.EndHorizontal();
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Description",GUILayout.Width(70));
            itemModel.Description.Key = GUILayout.TextArea(itemModel.Description.Key,GUILayout.Height(45));
          }
          GUILayout.EndHorizontal();
          GUILayout.Space(3);
          
        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete race?",
            "Do you want to delete: "+itemModel.Name,"Delete","Cancel"))
          {
            _itemsDb.Remove(itemModel);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
      EditorStatic.DrawThinLine(10);
     //RARITY:
     var raritiesDict = _itemsRaritiesDb.Db;
     if (raritiesDict.Any())
     {
         ItemRarity currentRarity = null;
         if (raritiesDict.ContainsKey(itemModel.RarityKey))
         {
             currentRarity = raritiesDict[itemModel.RarityKey];
         }
         else
         {
             currentRarity = raritiesDict.GetEnumerator().Current.Value;
         }



     }
     else
     {
         EditorGUILayout.HelpBox("No Rarities found in settings", MessageType.Warning);
     }

     EditorStatic.DrawThinLine(10);
    }
    }

}

