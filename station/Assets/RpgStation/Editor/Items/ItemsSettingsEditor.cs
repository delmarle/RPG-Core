using System;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static partial class ItemsEditor
    {
        #region fields 

        private static int _selectedEquipmentSlot;
        private static Vector2 _equipmentSlotsScrollPos;
        
        private static int _itemSettingsBarIndex;
        private static Vector2 _itemSettingsScrollPos;
        #endregion

        private static void DrawingSettings()
        {
            if (_itemsSettingsDb == null)
            {
                EditorGUILayout.HelpBox("cannot find item settings db", MessageType.Info);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                DrawSettingsBar();
         
                DrawItemSettingsProperties();

                EditorGUILayout.EndHorizontal();
            }

        }
        
        private static void DrawSettingsBar()
        {
            GUILayout.BeginVertical("box",GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH),GUILayout.ExpandHeight(true));
            {
                var  toolbarOptions = new GUIContent[6];
                toolbarOptions[0] = new GUIContent(EditorStatic.ITEMS_TAB_SETTINGS_TAGS,null, "");
                toolbarOptions[1] = new GUIContent(EditorStatic.ITEMS_TAB_SETTINGS_RARITIES,null, "");
                toolbarOptions[2] = new GUIContent(EditorStatic.ITEMS_TAB_SETTINGS_ITEMS_CATEGORIES,null, "");
                toolbarOptions[3] = new GUIContent(EditorStatic.ITEMS_TAB_SETTINGS_EQUIPMENT_SLOTS, null, "");
                toolbarOptions[4] = new GUIContent(EditorStatic.ITEMS_TAB_SETTINGS_CONTAINERS, null, "");
                toolbarOptions[5] = new GUIContent(EditorStatic.ITEMS_TAB_SETTINGS_CRAFTING, null, "");

                var height = 40 * toolbarOptions.Length;
                _itemSettingsBarIndex = GUILayout.SelectionGrid(_itemSettingsBarIndex, toolbarOptions,1,EditorStatic.ToolBarStyle,GUILayout.Height(height));
            }
            GUILayout.EndVertical();
        }
        
        private static void DrawItemSettingsProperties()
        {
            _itemSettingsScrollPos = GUILayout.BeginScrollView(_itemSettingsScrollPos,GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
            {
        
                switch (_itemSettingsBarIndex)
                {
                    case 0:
                        DrawTags();
                        break;
                    case 1:
                        DrawRarities();
                        break;
                    case 2:
                        DrawItemsCategories();
                        break;
                    case 3:
                        DrawEquipmentSettings();
                        break;
                    case 4:
                        DrawContainersSettings();
                        break;
                }
            }
            GUILayout.EndScrollView();
        }
        
        private static void DrawTags()
        {
            EditorStatic.DrawSectionTitle(55, "Items Tags");
            if (EditorStatic.SizeableButton(100, 32, "Add", "plus"))
            {
                _itemsSettingsDb.Get().ItemsTags.Add("new tag");
            }
            EditorStatic.DrawLargeLine();
            var tags = _itemsSettingsDb.Get().ItemsTags;
            for (var index = 0; index < tags.Count; index++)
            {
                var tag = tags[index];
                EditorGUILayout.BeginHorizontal();
                tags[index] = EditorGUILayout.TextField(tag);
                if (EditorStatic.SizeableButton(64, 18, "delete", "cross"))
                {
                    _itemsSettingsDb.Get().ItemsTags.Remove(tag);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawRarities()
        {
            EditorStatic.DrawSectionTitle(55, "Items Rarities");
            if (EditorStatic.SizeableButton(100, 32, "Add", "plus"))
            {
                
                _itemsRaritiesDb.Add(new ItemRarity());
            }
            EditorStatic.DrawLargeLine();
            var list = _itemsRaritiesDb.Db;
            foreach (var entry in list)
            {
                EditorGUILayout.BeginHorizontal();
                EditorStatic.DrawLocalization(entry.Value.Name);
                entry.Value.Color= EditorGUILayout.ColorField(entry.Value.Color);
                if (EditorStatic.SizeableButton(64, 18, "delete", "cross"))
                {
                    _itemsRaritiesDb.Remove(entry.Value);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawItemsCategories()
        {
            EditorStatic.DrawSectionTitle(55, "Items Categories");
            if (EditorStatic.SizeableButton(100, 32, "Add", "plus"))
            {
                _itemsCategoriesDb.Add(new ItemCategory());
            }
            EditorStatic.DrawLargeLine();
            var list = _itemsCategoriesDb.Db;
            foreach (var entry in list)
            {
                EditorGUILayout.BeginHorizontal();
                EditorStatic.DrawLocalization(entry.Value.Name);
                //    entry.Value= EditorGUILayout.ColorField(entry.Value.Color);
                if (EditorStatic.SizeableButton(64, 18, "delete", "cross"))
                {
                    _itemsCategoriesDb.Remove(entry.Value);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawEquipmentSettings()
        {
            EditorGUILayout.BeginHorizontal();
            _selectedEquipmentSlot = EditorStatic.DrawGenericSelectionList(_equipmentSlotsDb, _selectedEquipmentSlot,
                _equipmentSlotsScrollPos, out _equipmentSlotsScrollPos, "bullet_yellow", true);
            EditorGUILayout.BeginVertical("box");
            EditorStatic.DrawSectionTitle(55, "Equipment slots");
            EditorStatic.DrawLargeLine();
 
            var entry = _equipmentSlotsDb.GetEntry(_selectedEquipmentSlot);
            if (entry != null)
            {
                EditorGUILayout.BeginHorizontal("box");
                entry.Icon = (Sprite)EditorGUILayout.ObjectField(entry.Icon, typeof(Sprite), false, GUILayout.Width(64), GUILayout.Height(64));
                
                EditorGUILayout.BeginVertical();
                EditorStatic.DrawLocalization(entry.Name, "Slot name: ");
                EditorStatic.DrawLocalization(entry.Description, "Description: ");
                EditorGUILayout.EndVertical();
               
                if (EditorStatic.SizeableButton(90, 64, "delete", "cross"))
                {
                    _equipmentSlotsDb.Remove(entry);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("cannot find entries", MessageType.Info);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        

        private static void DrawContainersSettings()
        {
            EditorStatic.DrawSectionTitle(55, "Container settings");
            EditorStatic.DrawLargeLine();
            _itemsSettingsDb.Get().ContainerSettings.PlayerInventoryType = (PlayerInventoryType)EditorGUILayout.EnumPopup(
                "player inventory mode:",
                _itemsSettingsDb.Get().ContainerSettings.PlayerInventoryType);

            _itemsSettingsDb.Get().ContainerSettings.InitialPlayerInventorySize = EditorGUILayout.IntField(
                "Initial player inventory size:", _itemsSettingsDb.Get().ContainerSettings.InitialPlayerInventorySize);
            
            EditorStatic.DrawThinLine();
            _itemsSettingsDb.Get().ContainerSettings.ContainerPopup = 
                (UiPopup) EditorGUILayout.ObjectField("Container popup: ", _itemsSettingsDb.Get().ContainerSettings.ContainerPopup, typeof(UiPopup), true);
            _itemsSettingsDb.Get().ContainerSettings.CharacterLootPopup = 
                (UiPopup) EditorGUILayout.ObjectField("Character loot popup: ", _itemsSettingsDb.Get().ContainerSettings.CharacterLootPopup, typeof(UiPopup), true);
        }
    }

    
}
