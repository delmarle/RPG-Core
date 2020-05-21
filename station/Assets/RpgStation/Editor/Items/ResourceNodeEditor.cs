using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static partial class ItemsEditor
    {
        private const string RESOURCE_NODE_CATEGORY = "resource_node";
        private static int _selectedNodeEntry;
        private static Vector2 _nodeListPropertyScrollPos;
        private static Vector2 _nodeScrollPos;
        private static bool displayStartSound;
        private static bool displayStopSound;
        private static bool displayCollectSound = false;

        public static void DrawResourceNodeEditor()
        {
            if (_itemsDb == null)
            {
                EditorGUILayout.HelpBox("Items Db is missing", MessageType.Error);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                DrawResourceNodeList();
                DrawNodeView();
                EditorGUILayout.EndHorizontal();
            }
        }
        
        private static void DrawResourceNodeList()
        {
            _selectedNodeEntry = EditorStatic.DrawGenericSelectionList(_resourcesNodeDb, _selectedNodeEntry, _nodeListPropertyScrollPos,out _nodeListPropertyScrollPos,"package",false);
        }

        private static void DrawNodeView()
        {
            var nodeCount = _resourcesNodeDb.Count();
            if (_selectedNodeEntry == -1) return;
            if (nodeCount == 0) return;
            if (nodeCount < _selectedNodeEntry+1) _selectedNodeEntry = 0;
      
            var node = _resourcesNodeDb.GetEntry(_selectedNodeEntry);
            GUILayout.BeginHorizontal("box");
            {
                _nodeScrollPos = EditorGUILayout.BeginScrollView(_nodeScrollPos, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
                {
                    NodePanel(node,_selectedNodeEntry);
                   
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();
        }

        private static void NodePanel(ResourceNodeModel nodeModel, int selectedIndex)
        {
            GUILayout.Label("EDIT Node:", GUILayout.Width(70));
            EditorStatic.DrawLargeLine(5);
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                nodeModel.Icon =
                    (Sprite)EditorGUILayout.ObjectField(nodeModel.Icon,typeof(Sprite), false, GUILayout.Width(EditorStatic.EDITOR_BUTTON_SIZE),
                        GUILayout.Height(EditorStatic.EDITOR_BUTTON_SIZE));
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
                        _resourcesNodeDb.Remove(nodeModel);
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
            nodeModel.CycleLength = EditorGUILayout.FloatField("Time between each cycle: ", nodeModel.CycleLength);
            EditorStatic.DrawThinLine(5);
            displayStartSound = EditorStatic.SoundFoldout("Start sound: ", ref nodeModel.StartSound, displayStartSound, 28, Color.cyan);
            if (displayStartSound)
            {
                EditorStatic.DrawSoundWidget(ref  nodeModel.StartSound, RESOURCE_NODE_CATEGORY);
            }
            displayCollectSound = EditorStatic.SoundFoldout("Collect sound: ", ref nodeModel.CollectSound, displayCollectSound, 28, Color.cyan);
            if (displayCollectSound)
            {
                EditorStatic.DrawSoundWidget(ref  nodeModel.CollectSound, RESOURCE_NODE_CATEGORY);
            }
            displayStopSound = EditorStatic.SoundFoldout("Stop sound: ", ref nodeModel.StopSound, displayStopSound, 28, Color.cyan);
            if (displayStopSound)
            {
                EditorStatic.DrawSoundWidget(ref  nodeModel.StopSound, RESOURCE_NODE_CATEGORY);
            }
        }
    }
}