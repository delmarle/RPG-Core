using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class FootStepsTabEditor
    {
        private static FootstepsDb _db;
        private static int _toolBarIndex;
        private static int _selectedIndexGroup;
        private static Vector2 _propertyScrollPosLeftList;
        private static string _currentAssetName;
        private static List<bool> _statesFoldouts = new List<bool>();
        
        public static void DrawContent()
        {
            EditorGUI.BeginChangeCheck();
            {
                if (_db == null)
                {
                    _db = (FootstepsDb)EditorStatic.GetDb(typeof(FootstepsDb));
                    EditorGUILayout.HelpBox("Db is missing", MessageType.Warning);
                }
                else
                {
                    _toolBarIndex = GUILayout.Toolbar(_toolBarIndex, EditorStatic.FootStepsToolbarOptions, EditorStatic.ToolBarStyle, EditorStatic.ToolbarHeight);
                    switch (_toolBarIndex)
                    {
                        case 0:
                            DrawSettingsView();
                            break;
                        case 1:
                            DrawSurfaceView();
                            break;
                        case 2:
                            GUILayout.BeginHorizontal();
                            DrawTemplatesView();
                            GUILayout.EndHorizontal();
                            break;
                    }
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (_db != null)
                {
                    _db.ForceRefresh(); 
                }
            }
        }

        public static void DrawSettingsView()
        {
            EditorStatic.DrawSectionTitle(32, "FootstepSettings: ");
            _db.DefaultFootstepConfig = (SourcePoolConfig)EditorGUILayout.ObjectField("defaultFootstep config: ", _db.DefaultFootstepConfig, typeof(SourcePoolConfig), false);

            if (_db.DefaultFootstepConfig == null)
            {
                if (EditorStatic.SizeableButton(100, 32, "Create default", "plus"))
                {
                    _db.DefaultFootstepConfig =
                        ScriptableHelper.CreateScriptableObject<SourcePoolConfig>("Assets/Content/Audio sources/", "default_footstep_pool.asset");
                }
            }
            else
            {
                if (EditorStatic.SizeableButton(150, 32, "Apply to all templates", "cog"))
                {
                    foreach (var entry in _db.Db)
                    {
                        if (entry.Value != null)
                        {
                            foreach (var cf in entry.Value.Entries)
                            {
                                if (cf.Sounds != null)
                                {
                                    cf.Sounds.SourceConfig = _db.DefaultFootstepConfig;
                                }
                            }
                        }
                    }

                }
            }
        }
        
        public static void DrawSurfaceView()
        {
            EditorGUILayout.HelpBox("add the surfaces that would be unique sounds: grass, concrete, wood, metal...", MessageType.Info);

            if (EditorStatic.SizeableButton(100, 32, "Add", "plus"))
            {
                _db.Surfaces.Add("new surface");

            }
            EditorStatic.DrawLargeLine();
            var tags =  _db.Surfaces;
            for (var index = 0; index < tags.Count; index++)
            {
                var tag = tags[index];
                EditorGUILayout.BeginHorizontal();
                tags[index] = EditorGUILayout.TextField(tag);
                if (EditorStatic.SizeableButton(64, 18, "delete", "cross"))
                {
                    _db.Surfaces.Remove(tag);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        
        public static void DrawTemplatesView()
        {
            GUILayout.BeginVertical("box", GUILayout.Width(150), GUILayout.ExpandHeight(true));
            {
                EditorGUILayout.LabelField("groups: ");
                DrawSelectionList();
            }
            GUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorStatic.DrawSectionTitle(32, "Add a new footstep template: ");

            EditorGUILayout.BeginHorizontal();
            if (EditorStatic.SizeableButton(280, 45, "Add", "plus"))
            {
                string assetguid = Guid.NewGuid().ToString();
                var so = ScriptableHelper.CreateScriptableObject<FootSoundTemplate>(
                    EditorStatic.EDITOR_FOOTSTEPS_TEMPLATES_PATH,
                    assetguid + ".asset");
                AssetDatabase.SaveAssets();
                _db.Add(so);
            }

            var entryDb = _db.GetEntry(_selectedIndexGroup);


            if (entryDb == null)
            {
                EditorGUILayout.HelpBox("no group found", MessageType.Info);
            }
            else
            {
                if (EditorStatic.SizeableButton(280, 45, $"DELETE Template", "cross"))
                {
                    string assetPath = AssetDatabase.GetAssetPath(entryDb);
                    AssetDatabase.DeleteAsset(assetPath);
                    AssetDatabase.SaveAssets();

                    _db.Remove(entryDb);
                    _selectedIndexGroup = 0;
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorStatic.DrawLargeLine(5);

            if (entryDb != null)
            {
                EditorStatic.DrawSectionTitle(32, $"Edit template: {entryDb}: ");
                EditorGUILayout.BeginHorizontal();
                _currentAssetName = EditorGUILayout.TextField("name:", _currentAssetName);
                if (EditorStatic.SizeableButton(200, 32, "RENAME", ""))
                {
                    if (_currentAssetName.Length > 1)
                    {
                        ScriptableHelper.RenameScriptAbleAsset(entryDb, _currentAssetName);
                        _currentAssetName = string.Empty;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorStatic.DrawLargeLine(5);
                //
                if (_db.Surfaces.Count == 0)
                {
                    EditorGUILayout.HelpBox("no surfaces defined", MessageType.Warning);
                }
                else
                {
                    
                    for (var index = 0; index < _db.Surfaces.Count; index++)
                    {
                        var surface = _db.Surfaces[index];
                        if (entryDb.Entries.Count < index+1)
                        {
                            SurfaceEntry surfaceEntry = new SurfaceEntry();
                            entryDb.Entries.Add(surfaceEntry);
                        }
                        entryDb.Entries[index].SurfaceName = surface;
                        if (_statesFoldouts.Count < entryDb.Entries.Count)
                        {
                            _statesFoldouts.Add(false);
                        }

                        _statesFoldouts[index] = EditorStatic.SoundFoldout(surface, ref entryDb.Entries[index].Sounds,
                            _statesFoldouts[index], 28, Color.grey);
                        if (_statesFoldouts[index])
                        {
                            EditorStatic.DrawSoundWidget(ref entryDb.Entries[index].Sounds, "footsteps", $"{entryDb.name}_{entryDb.Entries[index].SurfaceName}_footstep");
                        }

       
                        //if null add button to create
                        EditorStatic.DrawThinLine(2);
                    }
                }

            }

            EditorGUILayout.EndVertical();
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
    }
}