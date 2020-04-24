using System;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static partial class ItemsEditor
    {
        #region fields 
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
                toolbarOptions[2] = new GUIContent(EditorStatic.ITEMS_TAB_SETTINGS_ITEMS_TYPES,null, "");
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
                        DrawItemsTypes();
                        break;
                    case 3:
                        DrawEquipmentSlots();
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
                _itemsSettingsDb.Get().ItemsRarities.Add(Guid.NewGuid().ToString() ,new ItemRarity());
            }
            EditorStatic.DrawLargeLine();
            var list = _itemsSettingsDb.Get().ItemsRarities;
            foreach (var entry in list)
            {
                EditorGUILayout.BeginHorizontal();
                entry.Value.Name = EditorGUILayout.TextField(entry.Value.Name);
                entry.Value.Color= EditorGUILayout.ColorField(entry.Value.Color);
                if (EditorStatic.SizeableButton(64, 18, "delete", "cross"))
                {
                    _itemsSettingsDb.Get().ItemsRarities.Remove(entry);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawItemsTypes()
        {
        }

        private static void DrawEquipmentSlots()
        {
            
            EditorStatic.DrawSectionTitle(55, "Items Rarities");
            if (EditorStatic.SizeableButton(100, 32, "Add", "plus"))
            {
                _itemsSettingsDb.Get().EquipmentSlots.Add(Guid.NewGuid().ToString() ,new EquipmentSlot());
            }
            EditorStatic.DrawLargeLine();
            var list = _itemsSettingsDb.Get().EquipmentSlots;
            foreach (var entry in list)
            {
                EditorGUILayout.BeginHorizontal();
                entry.Value.Name = EditorGUILayout.TextField(entry.Value.Name);
                entry.Value.Description = EditorGUILayout.TextField(entry.Value.Description);
               // entry.Value.Icon = (Sprite)EditorGUILayout.ObjectField(entry.Value.Icon);
                if (EditorStatic.SizeableButton(64, 18, "delete", "cross"))
                {
                    _itemsSettingsDb.Get().ItemsRarities.Remove(entry);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }

    
}
