using System;
using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Station
{
    public static class SoundTabEditor
    {
        
        private static SoundsDb _db;

        private static int _selectedIndexGroup;
        private static Vector2 _propertyScrollPosLeftList;

       // private static int _selectedIndex = 1;
        private static Vector2 _scrollPos;
        private static string NewGroupName;
        private static string NewSoundName;
        private static Dictionary<string, Dictionary<string, bool>> state = new Dictionary<string, Dictionary<string, bool>>();
        
        public static void DrawContent()
        {
            EditorGUI.BeginChangeCheck();
            {
                GUILayout.BeginHorizontal();
                Draw();
                GUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (_db != null)
                {
                    _db.ForceRefresh(); 
                }
            }
        }

        private static void Draw()
        {
            if (_db == null)
            {
                _db = (SoundsDb)EditorStatic.GetDb(typeof(SoundsDb));
                EditorGUILayout.HelpBox("Db is missing", MessageType.Warning);
            }
            else
            {
                AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(EditorStatic.EDITOR_ADDRESSABLE_PATH);
                if (settings == null)
                {
                    Debug.LogError("cant find db at: ");
                }

                GUILayout.BeginVertical("box",GUILayout.Width(150), GUILayout.ExpandHeight(true));
                {
                    EditorGUILayout.LabelField("groups: ");
                    DrawSelectionList();

                }
                GUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorStatic.DrawSectionTitle(32,"Add a new group: ");
                NewGroupName = EditorGUILayout.TextField(NewGroupName);
                EditorGUILayout.BeginHorizontal();
                if (EditorStatic.SizeableButton(120,45, "Add", "plus"))
                {
                    if (string.IsNullOrEmpty(NewGroupName))
                    {
                        return;
                    }

                    AddressableAssetGroup newGroup = null;
                    foreach (var addGroup in settings.groups)
                    {
                        if (addGroup.Name == EditorStatic.EDITOR_SOUND_GROUP_NAME )
                        {

                            newGroup = addGroup;
                        }
                    }

                    if (newGroup == null)
                    {
                        newGroup = settings.CreateGroup(EditorStatic.EDITOR_SOUND_GROUP_NAME, false, false, false, null);
                    }
                    
//Make a gameobject an addressable
                    ScriptableHelper.CreateScriptableObject<SoundContainer>(EditorStatic.EDITOR_SOUND_GROUP_PATH,
                        NewGroupName + ".asset");
                    AssetDatabase.SaveAssets();
                    
                    var guid = AssetDatabase.AssetPathToGUID(EditorStatic.EDITOR_SOUND_GROUP_PATH+NewGroupName+".asset");

//This is the function that actually makes the object addressable

                    var entry = settings.CreateOrMoveEntry(guid, newGroup);

                    entry.labels.Add(NewGroupName);

                    entry.address = NewGroupName;

//You'll need these to run to save the changes!

                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);

                    AssetDatabase.SaveAssets();



//////


                    _db.Add(new SoundCategory{CategoryName = NewGroupName});
                    NewGroupName = String.Empty;
                    
                }
                
                var entryDb = _db.GetEntry(_selectedIndexGroup);
                if (entryDb == null)
                {
                    EditorGUILayout.HelpBox("no group found", MessageType.Info);
                }
                else
                {
                    if (EditorStatic.SizeableButton(200,45, $"DELETE: [{entryDb.CategoryName}]", "cross"))
                    {
                        var template = settings.FindGroup(EditorStatic.EDITOR_SOUND_GROUP_NAME);
                        if (template)
                        {
                            settings.RemoveGroup(template);
                        }
                        AssetDatabase.DeleteAsset(EditorStatic.EDITOR_SOUND_GROUP_PATH+entryDb.CategoryName+".asset");
                        AssetDatabase.SaveAssets();

                        _db.Remove(entryDb);
                        _selectedIndexGroup = 0;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorStatic.DrawLargeLine(5);
                var currentGroup = _db.GetEntry(_selectedIndexGroup);
                if (currentGroup != null)
                {
                    if (state.ContainsKey(currentGroup.CategoryName) == false)
                    {
                        state.Add(currentGroup.CategoryName, new Dictionary<string, bool>());
                    }

                    string assetPath = EditorStatic.EDITOR_SOUND_GROUP_PATH + currentGroup.CategoryName + ".asset";
                    var groupAsset = AssetDatabase.LoadAssetAtPath<SoundContainer>(assetPath);
              
                    EditorGUILayout.LabelField($"Sounds: {groupAsset?.List?.Count}");
                    NewSoundName = EditorGUILayout.TextField("Sound name to add: ", NewSoundName);
                    if (EditorStatic.SizeableButton(200, 45, $"ADD: ", "plus"))
                    {
                        string fileName = $"{NewSoundName}_{Guid.NewGuid()}.asset";
                        var path = $"Assets/Content/Sounds/{currentGroup.CategoryName}/";
                        var sound = ScriptableHelper.CreateScriptableObject<SoundConfig>(path, fileName);
                        groupAsset.List.Add(sound);
                        AssetDatabase.SaveAssets();
                    }

                    foreach (var entry in groupAsset.List)
                    {
                        if (state[currentGroup.CategoryName].ContainsKey(entry.name) == false)
                        {
                            state[currentGroup.CategoryName].Add(entry.name, false);
                        }

                        var displayBool = state[currentGroup.CategoryName][entry.name];

                        var soundConfig = entry;
                        displayBool = EditorStatic.SoundFoldout("Open sound: ", ref soundConfig, displayBool, 28, Color.cyan);
                        if (displayBool)
                        {
                            DrawSoundWidget(entry, currentGroup.CategoryName);
                        }

                   
                        state[currentGroup.CategoryName][entry.name] = displayBool;
                    }
                }

                EditorGUILayout.EndVertical();
            }

        }

        public static void DrawSelectionList()
        {
            var entries = _db.ListEntryNames();
            GUILayout.BeginVertical(GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH), GUILayout.ExpandHeight(true));
            {
                _propertyScrollPosLeftList = GUILayout.BeginScrollView(_propertyScrollPosLeftList,
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    GUILayout.Label("SELECT " + _db.ObjectName() + ": ",
                        GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH - 30));
                    var toolbarOptions = new GUIContent[_db.Count()];
                    for (int i = 0; i < _db.Count(); i++)
                    {
                        toolbarOptions[i] = new GUIContent(entries[i], null, "");
                    }

                    var previousIndex = _selectedIndexGroup;

                    float lElemH2 = toolbarOptions.Length * 40;
                    _selectedIndexGroup = GUILayout.SelectionGrid(_selectedIndexGroup, toolbarOptions, 1,
                        EditorStatic.VerticalBarStyle, GUILayout.Height(lElemH2),
                        GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH - 10));
                    if (previousIndex != _selectedIndexGroup) EditorStatic.ResetFocus();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }

        public static void DrawSoundWidget(SoundConfig sound, string category)
        {
            var styleSubContent = new GUIStyle("SelectionRect");

            styleSubContent.padding = new RectOffset(3, 3, 6, 5);
            EditorGUILayout.BeginVertical(styleSubContent);
            if (sound)
            {
                EditorGUILayout.BeginHorizontal();
                sound.Volume = EditorGUILayout.Slider("Volume: ", sound.Volume, 0, 1);
                //delay
                sound.DelayAtStart = EditorGUILayout.Slider("Delay : ", sound.DelayAtStart, 0, 10);
                EditorGUILayout.EndHorizontal();

                // fade in
                EditorGUILayout.BeginHorizontal();
                sound.FadeInTime = EditorGUILayout.Slider("Fade In : ", sound.FadeInTime, 0, 10);
                sound.FadeOutTime = EditorGUILayout.Slider("Fade Out : ", sound.FadeOutTime, 0, 10);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.MinMaxSlider("Pitch : ", ref sound.MinPitch, ref sound.MaxPitch, 0, 2);
                sound.MinPitch = EditorGUILayout.FloatField(sound.MinPitch, GUILayout.Width(32));
                sound.MaxPitch = EditorGUILayout.FloatField(sound.MaxPitch, GUILayout.Width(32));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                sound.Looping = EditorGUILayout.Toggle("Looping : ", sound.Looping);
                sound.PoolSize = EditorGUILayout.IntField("Pool size : ", sound.PoolSize);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginVertical("box");
                //HEADER
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.Space();
                    if (EditorStatic.SizeableButton(300, 32, "ADD CLIP", "plus"))
                    {
                        sound.Clips.Add(null);
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Clips: " + sound.Clips.Count);
                }
                EditorGUILayout.EndHorizontal();

                if (sound.Clips.Count > 0)
                {
                    EditorStatic.DrawThinLine();
                    for (var index = 0; index < sound.Clips.Count; index++)
                    {
                        //LIST
                        EditorGUILayout.BeginHorizontal();
                        sound.Clips[index] = (AudioClip) EditorGUILayout.ObjectField("Clip : ", sound.Clips[index],
                            typeof(AudioClip), false);
                        if (EditorStatic.SizeableButton(100, 18, "DELETE", ""))
                        {
                            sound.Clips.RemoveAt(index);
                            GUIUtility.ExitGUI();
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorStatic.DrawThinLine();
                    }
                }

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("No sound asset created", MessageType.Warning);
                EditorGUILayout.Space();
                if (EditorStatic.SizeableButton(200, 36, "Create sound asset", "plus"))
                {
                    string fileName = category + Guid.NewGuid() + ".asset";
                    var path = PathUtils.BuildSoundPath(category);
                    sound = ScriptableHelper.CreateScriptableObject<SoundConfig>(path, fileName);
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();
        }
    }
}