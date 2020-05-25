using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static partial class ItemsEditor
    {
        private const string CHEST_NODE_CATEGORY = "resource_node";
        private static int _selectedChestEntry;
        private static Vector2 _chestListPropertyScrollPos;
        private static Vector2 _chestScrollPos;
        private static bool displayOpenChestSound;
        private static bool displayCloseSound;


        public static void DrawChestNodeEditor()
        {
            if (_chestNodesDb == null)
            {
                EditorGUILayout.HelpBox("_chestNodesDb Db is missing", MessageType.Error);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                DrawChestNodeList();
                DrawChestNodeView();
                EditorGUILayout.EndHorizontal();
            }
        }
        
        private static void DrawChestNodeList()
        {
            _selectedChestEntry = EditorStatic.DrawGenericSelectionList(_chestNodesDb, _selectedChestEntry, _chestListPropertyScrollPos,out _chestListPropertyScrollPos,"package",false);
        }

        private static void DrawChestNodeView()
        {
            var nodeCount = _chestNodesDb.Count();
            if (_selectedChestEntry == -1) return;
            if (nodeCount == 0) return;
            if (nodeCount < _selectedChestEntry+1) _selectedChestEntry = 0;
      
            var node = _chestNodesDb.GetEntry(_selectedChestEntry);
            GUILayout.BeginHorizontal("box");
            {
                _chestScrollPos = EditorGUILayout.BeginScrollView(_chestScrollPos, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
                {
                    ChestNodePanel(node,_selectedChestEntry);
                   
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();
        }

        private static void ChestNodePanel(ChestNodeModel nodeModel, int selectedIndex)
        {
            GUILayout.Label("EDIT Node:", GUILayout.Width(70));
            EditorStatic.DrawLargeLine(5);
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                nodeModel.Icon = (Sprite)EditorGUILayout.ObjectField(nodeModel.Icon,typeof(Sprite), false, GUILayout.Width(EditorStatic.EDITOR_BUTTON_SIZE), GUILayout.Height(EditorStatic.EDITOR_BUTTON_SIZE));
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Name", GUILayout.Width(70));
                        nodeModel.Name.Key = GUILayout.TextField(nodeModel.Name.Key);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Description", GUILayout.Width(70));
                        nodeModel.Description.Key = GUILayout.TextArea(nodeModel.Description.Key, GUILayout.Height(45));
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(3);
                }
                GUILayout.EndVertical();
                if (EditorStatic.ButtonDelete())
                {
                    if (EditorUtility.DisplayDialog("Delete race?",
                        "Do you want to delete: " + nodeModel.Name, "Delete", "Cancel"))
                    {
                        _chestNodesDb.Remove(nodeModel);
                        EditorStatic.ResetFocus();
                        return;
                    }
                }
            }
            GUILayout.EndHorizontal();
            
            EditorStatic.DrawThinLine(5);
            EditorStatic.DrawSectionTitle("Loots", 0);
            if (EditorStatic.SizeableButton(200, 32,"Add One", "plus"))
            {
                nodeModel.Loots.Add(new LootModel());
            }
            EditorStatic.DrawLargeLine(5);
            if (_itemsDb.Count() == 0)
            {
                EditorGUILayout.HelpBox("there is no items in the items db", MessageType.Info);
            }
            else
            {
                foreach (var entryLoot in nodeModel.Loots)
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
                        nodeModel.Loots.Remove(entryLoot);
                        GUIUtility.ExitGUI();
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorStatic.DrawSectionTitle("Node base data", 0);
         
            EditorStatic.DrawThinLine(5);
            nodeModel.Prefab = (ChestNode) EditorGUILayout.ObjectField("Prefab: ", nodeModel.Prefab, typeof(ChestNode), false);
            displayOpenChestSound = EditorStatic.SoundFoldout("Open sound: ", ref nodeModel.OpenSound, displayOpenChestSound, 28, Color.cyan);
            if (displayOpenChestSound)
            {
                EditorStatic.DrawSoundWidget(ref  nodeModel.OpenSound, CHEST_NODE_CATEGORY);
            }
           
            displayCloseSound = EditorStatic.SoundFoldout("Close sound: ", ref nodeModel.CloseSound, displayCloseSound, 28, Color.cyan);
            if (displayCloseSound)
            {
                EditorStatic.DrawSoundWidget(ref  nodeModel.CloseSound, CHEST_NODE_CATEGORY);
            }
        }
    }
}