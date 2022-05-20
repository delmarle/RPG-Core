using System.Linq;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
     public class FactionSettingsEditor
    {
        private static int _toolBarIndex;
        private static FactionSettingsDb _dbSettings;
        private static FactionDb _db;

        public static void DrawSettings()
        {
            if (_db == null)
            {
                _db = (FactionDb)EditorStatic.GetDb(typeof(FactionDb));
            }
            if (_dbSettings == null)
            {
                _dbSettings = (FactionSettingsDb)EditorStatic.GetDb(typeof(FactionSettingsDb));
            }
            GUILayout.BeginVertical("box",GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));
            {
                var factions = _db.ListEntryNames();
                if (factions.Any())
                {
                 
                    int factionIndex = 0;
                    if (_dbSettings.Get().DefaultPlayerFaction != "")
                    {
                        factionIndex = _db.GetIndex(_dbSettings.Get().DefaultPlayerFaction);
                        if (factionIndex == -1)
                        {
                            factionIndex = 0;
                        }
                    }

                    factionIndex = EditorGUILayout.Popup("Player Default faction: ",factionIndex, factions);

                    _dbSettings.Get().DefaultPlayerFaction = _db.GetKey(factionIndex);
                }
                else
                {
                    EditorGUILayout.HelpBox(" Add at least one faction", MessageType.Warning);
                }

                
                
            }
            GUILayout.EndVertical();
        }

        public static void DrawContent()
        { 
            EditorGUI.BeginChangeCheck();
            {
                Draw();
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (_dbSettings != null)
                {
                    _dbSettings.ForceRefresh(); 
                }
            }
        }

        private static void Draw()
        {
            if (_dbSettings == null)
            {
                _dbSettings = (FactionSettingsDb)EditorStatic.GetDb(typeof(FactionSettingsDb));
            }

            
           
            GUILayout.BeginVertical("box",GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));
            {
               
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                int ranksCount = _dbSettings.Get().Ranks.Count;
                var  toolbarOptions = new GUIContent[ranksCount];
                int currentRank = 0;
                foreach (var rank in _dbSettings.Get().Ranks)
                {
                    toolbarOptions[currentRank] = new GUIContent(rank.Name+ " {"+rank.Value+"}",null, "");
                    currentRank++;
                }
               
               
                var height = 40 * toolbarOptions.Length;
                _toolBarIndex = GUILayout.SelectionGrid(_toolBarIndex, toolbarOptions,1,EditorStatic.ToolBarStyle,GUILayout.Height(height));
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical("box",GUILayout.ExpandHeight(true));
            {
              
                EditorGUILayout.LabelField("Selected faction rank:",EditorStatic.SectionTitle, GUILayout.Height(25));
                EditorGUILayout.Space();
                if (EditorStatic.SizeableButton(200,45,"Add positive rank", "plus"))
                {
                    FactionRank rank = new FactionRank
                    {
                        Name = "rank_"+ Random.Range(0,1000),
                        Value = 0
                    };
                    _dbSettings.Get().Ranks.Insert(0,rank);
                    _toolBarIndex = 0;
                }
                if (EditorStatic.SizeableButton(200,45,"Add Negative rank", "plus"))
                {
                    FactionRank rank = new FactionRank
                    {
                        Name = "rank_"+ Random.Range(0,1000),
                        Value = 0
                    };
                    _dbSettings.Get().Ranks.Add(rank);
                    _toolBarIndex = _dbSettings.Get().Ranks.Count-1;
                }

                if (_dbSettings.Get().Ranks.Any())
                {
                    if (_dbSettings.Get().Ranks.Count <= _toolBarIndex)
                    {
                        _toolBarIndex = _dbSettings.Get().Ranks.Count - 1;
                    }
                    EditorGUILayout.Space();
                    _dbSettings.Get().Ranks[_toolBarIndex].Name = EditorGUILayout.TextField("name ",_dbSettings.Get().Ranks[_toolBarIndex].Name);
                    _dbSettings.Get().Ranks[_toolBarIndex].Value = EditorGUILayout.IntField("range ",_dbSettings.Get().Ranks[_toolBarIndex].Value);
                    EditorGUILayout.Space();
                }

                

                if (EditorStatic.SizeableButton(140, 45, "     Move rank up", "bullet_arrow_up"))
                {
                    GUI.SetNextControlName("");
                    _dbSettings.Get().MoveEntryUp(_toolBarIndex);
                    if (_toolBarIndex > 0)
                    {
                        _toolBarIndex--;
                    }
                }
                if (EditorStatic.SizeableButton(140, 45, " Move rank down", "bullet_arrow_down"))
                {
                    _dbSettings.Get().MoveEntryDown(_toolBarIndex);
                    if(_toolBarIndex<_dbSettings.Get().Ranks.Count-1)
                        _toolBarIndex++;
                }
                if (EditorStatic.SizeableButton(140, 45, " Delete rank", "cross"))
                {
                    _dbSettings.Get().Ranks.RemoveAt(_toolBarIndex);
                    if (_dbSettings.Get().Ranks.Count < _toolBarIndex)
                    {
                        _toolBarIndex = _dbSettings.Get().Ranks.Count - 2;
                    }
                    return;
                }
                

            }
            GUILayout.EndVertical();
            
        }

    }
}

