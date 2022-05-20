using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class FloatingPopupEditor 
    {
        private static FloatingPopupDb _db;

        private static Vector2 _propertyScrollPos;
        private static int _selectedIndex = 1;
        private static Vector2 _scrollPos;

        public static void DrawContent()
        {
            EditorGUI.BeginChangeCheck();
            {
                Draw();
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (_db != null)
                {
                    _db.ForceRefresh(); 
                }
            }
        }
    
        #region [[ DRAW ]]

        private static void Draw()
        {
            if (_db == null)
            {
                _db = (FloatingPopupDb)EditorStatic.GetDb(typeof(FloatingPopupDb));
                EditorGUILayout.HelpBox("Db is missing", MessageType.Warning);
            }
            else
            {
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                {
                    DrawBar();
                     ListView(_selectedIndex);
                }
                GUILayout.EndHorizontal();
            }

           
        }

        private static void DrawBar()
        {
            _selectedIndex = EditorStatic.DrawGenericSelectionList(_db, _selectedIndex, _propertyScrollPos,out _propertyScrollPos,"bullet_yellow",false);
        }
        
        private static void ListView(int selected)
        {
            var raceCount = _db.Count();
            if (selected == -1) return;
            if (raceCount == 0) return;
            if (raceCount < selected+1) selected = 0;
      
            var entry = _db.GetEntry(selected);
            GUILayout.BeginHorizontal("box");
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
                {
                    DrawPanel(entry,selected);
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();
        }
        
          private static void DrawPanel(FloatingPopupModel staticData,int selected)
    {
      GUILayout.Label("EDIT Popup:",GUILayout.Width(70));
      EditorStatic.DrawLargeLine(5);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
 
        
        GUILayout.BeginVertical();
        {
          GUILayout.Space(5);
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Name: ",GUILayout.Width(70));
            staticData.Name = GUILayout.TextField(staticData.Name);
          }
          GUILayout.EndHorizontal();
          GUILayout.Space(3);
          
          staticData.Prefab = (FloatingPopup)EditorGUILayout.ObjectField("Prefab: ", staticData.Prefab, typeof(FloatingPopup), false);
          staticData.PoolSize = EditorGUILayout.IntSlider("Pool size: ", staticData.PoolSize, 1, 64);

        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete data?",
            "Do you want to delete: "+staticData.Name,"Delete","Cancel"))
          {
            _db.Remove(staticData);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
      EditorStatic.DrawThinLine(10);
      GUILayout.Label("RULES:",GUILayout.Width(70));

      DrawShowRule(staticData.Rule.LeaderRules, "Leader Rules:");
      EditorStatic.DrawThinLine(2);
      DrawShowRule(staticData.Rule.TeamMemberRules, "Team members Rules:");
      EditorStatic.DrawThinLine(2);
      DrawShowRule(staticData.Rule.NpcRules, "Npc Rules:");
      EditorStatic.DrawThinLine(2);
      DrawShowRule(staticData.Rule.PetRules, "Pet Rules:");
      EditorStatic.DrawThinLine(2);
      
      staticData.Rule.ShowOnlyVisible = EditorGUILayout.Toggle("Show only visible", staticData.Rule.ShowOnlyVisible);
      staticData.Rule.ShowRangeLimit = EditorGUILayout.FloatField("Limit Range", staticData.Rule.ShowRangeLimit);
    }
        #endregion
        private static void DrawShowRule(ShowRuleEntry rule, string ruleName)
        {
            GUILayout.Label(ruleName,GUILayout.Width(70));
            rule.ByAny = EditorGUILayout.Toggle("By Any", rule.ByAny);
            if (rule.ByAny == false)
            {
                rule.ByLeader = EditorGUILayout.Toggle("By Leader", rule.ByLeader);
                rule.ByTeamMember = EditorGUILayout.Toggle("By Team member", rule.ByTeamMember);
                rule.ByNpc = EditorGUILayout.Toggle("By Npc", rule.ByNpc);
                rule.ByPet = EditorGUILayout.Toggle("By Pet", rule.ByPet);
            }
        }
    }
    
    
}

