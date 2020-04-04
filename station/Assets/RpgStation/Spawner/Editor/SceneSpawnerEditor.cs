using System.Collections.Generic;
using RPG.Editor;
using Station;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CustomEditor(typeof(SceneSpawner))]
[CanEditMultipleObjects]
public class SceneSpawnerEditor : Editor 
{
    public List<bool> _entriesStates = new List<bool>();
    private NpcDb _npcDb;

    private void Cache()
    {
        _npcDb = (NpcDb) EditorStatic.GetDb(typeof(NpcDb));
    }

    public override void OnInspectorGUI()
    {
        Cache();
        EditorGUI.BeginChangeCheck ();

        var spawner = (SceneSpawner)target;
        SetStatesSizes(spawner);
      //  base.DrawDefaultInspector();
        spawner.InitMode = (InitMode)EditorGUILayout.EnumPopup("Initialization mode: ", spawner.InitMode);
        if (spawner.InitMode == InitMode.SAVED)
        {
            spawner.SpawnId = EditorGUILayout.TextField("save id: ", spawner.SpawnId);
        }

        spawner.entitiesSelectionMode = (EntitiesSelectionMode)EditorGUILayout.EnumPopup("Entities selection mode: ", spawner.entitiesSelectionMode);
        if (spawner.entitiesSelectionMode == EntitiesSelectionMode.RANDOM_FROM_AMOUNT)
        {
            spawner.SpawnAmount = EditorGUILayout.IntField("Spawn amount:", spawner.SpawnAmount);
        }

        spawner.ReSpawnMode = (ReSpawnMode)EditorGUILayout.EnumPopup("Respawn mode: ", spawner.ReSpawnMode);
        
        EditorStatic.DrawThinLine(2);
        EditorGUILayout.BeginHorizontal();
        if (EditorStatic.Button(true, 32, "Add", "plus"))
        {
            spawner.DataList.Add(new SpawnData());
        }
        if (EditorStatic.Button(true, 32, "Reset", "cog"))
        {
        }
        EditorGUILayout.EndHorizontal();
        EditorStatic.DrawThinLine(2);
        for (var index = 0; index < spawner.DataList.Count; index++)
        {
            DrawEntry(index, spawner);
        }
        
        if(EditorGUI.EndChangeCheck ()) {
            // This code will unsave the current scene if there's any change in the editor GUI.
            // Hence user would forcefully need to save the scene before changing scene
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    private void SetStatesSizes(SceneSpawner spawner)
    {

        while (_entriesStates.Count < spawner.DataList.Count)
        {
            _entriesStates.Add(false);
        }
    }
    
    private static bool LevelFoldout(string title, bool display)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.label).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 22;
        style.contentOffset = new Vector2(20f, -2f);

        var rect = GUILayoutUtility.GetRect(16f, 22f, style);
        GUI.Box(rect, title, style);
        var e = Event.current;

        var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        if (e.type == EventType.Repaint) {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) {
            display = !display;
            e.Use();
        }

        return display;
    }

    private void DrawEntry(int index, SceneSpawner spawner)
    {
        var entry = spawner.DataList[index];
        var highestWeight = HighestWeight(spawner);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        string description = entry.SpawnType+": "+entry.Weight+"/"+ highestWeight;
        _entriesStates[index] = LevelFoldout(description, _entriesStates[index]);
        if (EditorStatic.SizeableButton(20, 16, "X", ""))
        {
            spawner.DataList.RemoveAt(index);

            return;
        }

        EditorGUILayout.EndHorizontal();
        if (_entriesStates[index])
        {
            entry.SpawnType = (SpawnObjectType)EditorGUILayout.EnumPopup("Type: ", entry.SpawnType);
        
        
            if (entry.SpawnType == SpawnObjectType.NPC)
            {
                if (_npcDb.Count() == 0)
                {
                    EditorGUILayout.HelpBox("No npc found in the DB", MessageType.Warning);
                }
                else
                {
                    var foundId = _npcDb.GetIndex(entry.ObjectId);
                    if (foundId < 0)
                    {
                        foundId = 0;
                    }

                    foundId = EditorGUILayout.Popup("Selected: ", foundId, _npcDb.ListEntryNames());
                    entry.ObjectId = _npcDb.GetKey(foundId);
                }
            }

            if (entry.SpawnType == SpawnObjectType.ITEM)
            {
                // EditorGUILayout.LabelField("ITEM");
            }

            if (entry.SpawnType == SpawnObjectType.PREFAB)
            {
                entry.Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab: ", entry.Prefab, typeof(GameObject), false);

            }

            entry.Weight = EditorGUILayout.IntSlider("Weight: ", entry.Weight, 0, 999);
            entry.Position = (PositionProvider)EditorGUILayout.ObjectField("Position: ", entry.Position, typeof(PositionProvider), true);
        }


       
    }
    private int HighestWeight(SceneSpawner spawner)
    {
        int highest = 0;
        foreach (var entry in spawner.DataList)
        {
            if (entry.Weight > highest)
            {
                highest = entry.Weight;
            }
        }
        return highest;
    }
}
