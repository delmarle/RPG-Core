using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    
    public static class StatisticEditor
    {
        private static Vector2 _scrollPos;
        private static Texture2D _selectedTexture;
        private static bool _pickingTexture;
        private static Vector2 _propertyScrollPos;
        private static int _selectedIndex = 1;
        private static AttributesDb _attributesDb;
        private static VitalsDb _vitalsDb;
        private static StatisticDb _statisticsDb;

        private static bool CacheDbs()
        {
          bool missing = false;
          if (_attributesDb == null)
          {
            _attributesDb = (AttributesDb)EditorStatic.GetDb(typeof(AttributesDb));
            missing = true;
          }
          if (_statisticsDb == null)
          {
            _statisticsDb = (StatisticDb)EditorStatic.GetDb(typeof(StatisticDb));
            missing = true;
          }
          if (_vitalsDb == null)
          {
            _vitalsDb = (VitalsDb)EditorStatic.GetDb(typeof(VitalsDb));
            missing = true;
          }

          return missing;
        }
        
        public static void DrawContent()
        {  
            EditorGUI.BeginChangeCheck();
            {
              if (CacheDbs() == false)
              {
                DrawVitalsList();
                ListView(_selectedIndex);
              }
              else
              {
                EditorGUILayout.HelpBox("DBs are missing", MessageType.Warning);
              }

             
            }
            if (EditorGUI.EndChangeCheck())
            {
              _statisticsDb.ForceRefresh();
            }
        }
    
        #region [[ DRAW ]]

        private static void DrawVitalsList()
        {
            _selectedIndex = EditorStatic.DrawGenericSelectionList(_statisticsDb, _selectedIndex, _propertyScrollPos,out _propertyScrollPos,"bullet_green",false);
        }
        
        private static void ListView(int selectedStat)
    {
      var statsCount = _statisticsDb.Count();
      if (selectedStat == -1) return;
      if (statsCount == 0) return;
      if (statsCount < selectedStat+1) selectedStat = 0;
      
      var stat = _statisticsDb.GetEntry(selectedStat);
      GUILayout.BeginHorizontal("box");
      {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
        {
          StatPanel(stat,selectedStat);
        }
        EditorGUILayout.EndScrollView();
      }
      GUILayout.EndHorizontal();
    }

    private static void StatPanel(StatisticModel stat,int selectedStat)
    {
      string statId = _statisticsDb.GetKey(selectedStat);
      stat.Id = statId;
      GUILayout.Label("EDIT Statistic:",GUILayout.Width(90));
      EditorStatic.DrawLargeLine(5);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
        if (stat.Icon)
          _selectedTexture = stat.Icon.texture;
        else
          _selectedTexture = null;

        if (GUILayout.Button(_selectedTexture, GUILayout.Width(EditorStatic.EDITOR_BUTTON_SIZE),GUILayout.Height(EditorStatic.EDITOR_BUTTON_SIZE)))
        {
          int controllerId = GUIUtility.GetControlID(FocusType.Passive);
          EditorGUIUtility.ShowObjectPicker<Sprite>(null,false,null,controllerId);
          _pickingTexture = true;
        }
        string commandName = Event.current.commandName;
        if (_pickingTexture && commandName == EditorStatic.EDITOR_OBJECT_PICKER_COMMAND_NAME)
        {
          _statisticsDb.GetEntry(selectedStat).Icon = EditorGUIUtility.GetObjectPickerObject() as Sprite;
          _pickingTexture = false;
        }
        
        GUILayout.BeginVertical();
        {
          GUILayout.Space(5);
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Name",GUILayout.Width(70));
            stat.Name = GUILayout.TextField(stat.Name);
          }
          GUILayout.EndHorizontal();
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Description",GUILayout.Width(70));
            stat.Description = GUILayout.TextArea(stat.Description,GUILayout.Height(45));
          }
          GUILayout.EndHorizontal();
          
        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete statistic?",
            "Do you want to delete: "+stat.Name,"Delete","Cancel"))
          {
            _statisticsDb.Remove(stat);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
      EditorStatic.DrawThinLine(10);
      stat.Color = EditorGUILayout.ColorField("Color ",stat.Color,GUILayout.ExpandWidth(true));
      GUILayout.Space(3);
      stat.MinimumValue = EditorGUILayout.FloatField("Minimum value",stat.MinimumValue,GUILayout.ExpandWidth(true));
      GUILayout.Space(3);
      stat.MaximumValue = EditorGUILayout.FloatField("Maximum value",stat.MaximumValue,GUILayout.ExpandWidth(true));
      EditorStatic.DrawThinLine(10);
    }
        #endregion
    }
}

