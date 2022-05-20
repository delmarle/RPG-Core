using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static partial class ItemsEditor
    {
        #region [[FIELDS]]
        private static int _toolBarIndex;
        private static Vector2 _propertyScrollPos;

        //DBS
        private static ItemsCategoriesDb _itemsCategoriesDb;
        private static ItemsRaritiesDb _itemsRaritiesDb;
        private static ItemsSettingsDb _itemsSettingsDb;
        private static EquipmentSlotsDb _equipmentSlotsDb;
        private static EquipmentTypesDb _equipmentTypesDb;
        private static ItemsDb _itemsDb;
        private static ResourcesNodeDb _resourcesNodeDb;
        private static ChestNodesDb _chestNodesDb;
        #endregion
        private static void CacheDbs()
        {
            if (_chestNodesDb == null)
            {
                _chestNodesDb = (ChestNodesDb) EditorStatic.GetDb(typeof(ChestNodesDb));
                if (_chestNodesDb == null)
                {
                    EditorGUILayout.HelpBox("MISSING DB: _chestNodesDb", MessageType.Error);
                    GUIUtility.ExitGUI();
                }
            }
            
            if (_resourcesNodeDb == null)
            {
                _resourcesNodeDb = (ResourcesNodeDb) EditorStatic.GetDb(typeof(ResourcesNodeDb));
                if (_resourcesNodeDb == null)
                {
                    EditorGUILayout.HelpBox("MISSING DB: _equipmentTypesDb", MessageType.Error);
                    GUIUtility.ExitGUI();
                }
            }
            
            if (_equipmentTypesDb == null)
            {
                _equipmentTypesDb = (EquipmentTypesDb) EditorStatic.GetDb(typeof(EquipmentTypesDb));
                if (_equipmentTypesDb == null)
                {
                    EditorGUILayout.HelpBox("MISSING DB: _equipmentTypesDb", MessageType.Error);
                    GUIUtility.ExitGUI();
                }
            }
            
            if (_equipmentSlotsDb == null)
            {
                _equipmentSlotsDb = (EquipmentSlotsDb) EditorStatic.GetDb(typeof(EquipmentSlotsDb));
                if (_equipmentSlotsDb == null)
                {
                    EditorGUILayout.HelpBox("MISSING DB: _equipmentSlotsDb", MessageType.Error);
                    GUIUtility.ExitGUI();
                }
            }
            
            if (_itemsCategoriesDb == null)
            {
                _itemsCategoriesDb = (ItemsCategoriesDb) EditorStatic.GetDb(typeof(ItemsCategoriesDb));
                if (_itemsCategoriesDb == null)
                {
                    EditorGUILayout.HelpBox("MISSING DB: ItemsCategoriesDb", MessageType.Error);
                    GUIUtility.ExitGUI();
                }
            }
            if (_itemsSettingsDb == null)
            {
                _itemsSettingsDb = (ItemsSettingsDb) EditorStatic.GetDb(typeof(ItemsSettingsDb));
                if (_itemsSettingsDb == null)
                {
                    EditorGUILayout.HelpBox("MISSING DB: ItemsSettings", MessageType.Error);
                    GUIUtility.ExitGUI();
                }
            }
            if (_itemsDb == null)
            {
                _itemsDb = (ItemsDb) EditorStatic.GetDb(typeof(ItemsDb));
                if (_itemsDb == null)
                {
                    EditorGUILayout.HelpBox("MISSING DB: ItemsDb", MessageType.Error);
                    GUIUtility.ExitGUI();
                }
            }
            
            if (_itemsRaritiesDb == null)
            {
                _itemsRaritiesDb = (ItemsRaritiesDb) EditorStatic.GetDb(typeof(ItemsRaritiesDb));
                if (_itemsRaritiesDb == null)
                {
                    EditorGUILayout.HelpBox("MISSING DB: ItemsRaritiesDb", MessageType.Error);
                    GUIUtility.ExitGUI();
                }
                
            }
        }
        

        public static void DrawTab()
        {
            CacheDbs();
            EditorGUI.BeginChangeCheck();
            {
                GUILayout.BeginHorizontal();
                DrawLeftBar();
                DrawProperties();
                GUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                _itemsCategoriesDb?.ForceRefresh();
                _itemsRaritiesDb?.ForceRefresh();
                _itemsSettingsDb?.ForceRefresh();
                _equipmentSlotsDb?.ForceRefresh();
                _itemsDb?.ForceRefresh();
                _equipmentTypesDb?.ForceRefresh();
                _resourcesNodeDb?.ForceRefresh();
                _chestNodesDb?.ForceRefresh();
            }
        }
        
        private static void DrawLeftBar()
        {
            GUILayout.BeginVertical("box",GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH),GUILayout.ExpandHeight(true));
            {
                var  toolbarOptions = new GUIContent[8];
                toolbarOptions[0] = new GUIContent(EditorStatic.ITEMS_TAB_SETTINGS,null, "");
                toolbarOptions[1] = new GUIContent(EditorStatic.ITEMS_TAB_ITEMS,null, "");
                toolbarOptions[2] = new GUIContent(EditorStatic.ITEMS_TAB_LOOT_TABLES, null, "");
                toolbarOptions[3] = new GUIContent(EditorStatic.ITEMS_TAB_CURRENCIES, null, "");
                toolbarOptions[4] = new GUIContent(EditorStatic.ITEMS_TAB_SHOPS, null, "");
                toolbarOptions[5] = new GUIContent(EditorStatic.ITEMS_TAB_CRAFT, null, "");
                toolbarOptions[6] = new GUIContent(EditorStatic.ITEMS_TAB_RESOURCES_NODES, null, "");
                toolbarOptions[7] = new GUIContent(EditorStatic.ITEMS_TAB_CHEST_NODES, null, "");
                var height = 40 * toolbarOptions.Length;
                _toolBarIndex = GUILayout.SelectionGrid(_toolBarIndex, toolbarOptions,1,EditorStatic.ToolBarStyle,GUILayout.Height(height));
            }
            GUILayout.EndVertical();
        }

        private static void DrawProperties()
        {
            _propertyScrollPos = GUILayout.BeginScrollView(_propertyScrollPos,GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
            {
        
                switch (_toolBarIndex)
                {
                    case 0:
                        DrawingSettings();
                        break;
                    case 1:
                        DrawItems();
                        break;
                    case 2:
                        LootTableEditor.Draw();
                        break;
                    case 3:

                        break;
                    case 6:
                        DrawResourceNodeEditor();
                        break;
                    case 7:
                        DrawChestNodeEditor();
                        break;
                }
            }
            GUILayout.EndScrollView();
        }
    }
}

