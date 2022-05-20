using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class AttributesEditor 
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
          if (CacheDbs() == false)
          {
            EditorGUI.BeginChangeCheck();
            {
              AssignIds();
              DrawAttributesList();
              ListView(_selectedIndex);
            }
            if (EditorGUI.EndChangeCheck())
            {
              _attributesDb.ForceRefresh(); 
            }
          }
          else
          {
            EditorGUILayout.HelpBox("DBs are missing", MessageType.Warning);
          }

        }
    
        #region [[ DRAW ]]

        private static void DrawAttributesList()
        {
            _selectedIndex = EditorStatic.DrawGenericSelectionList(_attributesDb, _selectedIndex, _propertyScrollPos,out _propertyScrollPos,"bullet_yellow",false);
        }
        #endregion

        private static void AssignIds()
        {
          foreach (var attribute in _attributesDb.Db)
          {
            attribute.Value.Id = attribute.Key;
          }
        }
        
        private static void ListView(int selectedEntry)
    {
      var entriesCount = _attributesDb.Count();
      if (selectedEntry == -1) return;
      if (entriesCount == 0) return;
      if (entriesCount < selectedEntry+1) selectedEntry = 0;
      
      var entry = _attributesDb.GetEntry(selectedEntry);
      GUILayout.BeginHorizontal("box");
      {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
        {
          DrawPanel(entry,selectedEntry);
        }
        EditorGUILayout.EndScrollView();
      }
      GUILayout.EndHorizontal();
    }

    private static void DrawPanel(AttributeModel attributeStaticData,int selectedEntry)
    {
      GUILayout.Label("EDIT Attribute:",GUILayout.Width(90));
      EditorStatic.DrawLargeLine(5);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
        if (attributeStaticData.Icon)
          _selectedTexture = attributeStaticData.Icon.texture;
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
          _attributesDb.GetEntry(selectedEntry).Icon = EditorGUIUtility.GetObjectPickerObject() as Sprite;
          _pickingTexture = false;
        }
        
        GUILayout.BeginVertical();
        {
          GUILayout.Space(5);
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Name",GUILayout.Width(70));
            attributeStaticData.Name = GUILayout.TextField(attributeStaticData.Name);
          }
          GUILayout.EndHorizontal();
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Description",GUILayout.Width(70));
            attributeStaticData.Description = GUILayout.TextArea(attributeStaticData.Description,GUILayout.Height(45));
          }
          GUILayout.EndHorizontal();
          
        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete Attribute?",
            "Do you want to delete: "+attributeStaticData.Name,"Delete","Cancel"))
          {
            _attributesDb.Remove(attributeStaticData);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
      EditorStatic.DrawThinLine(10);
      attributeStaticData.DefaultValue = EditorGUILayout.IntField("Default value ",attributeStaticData.DefaultValue,GUILayout.ExpandWidth(true));
      EditorStatic.DrawThinLine(10);
      if (_vitalsDb.Count() > 0)
      {
        EditorStatic.DrawBonusWidget( attributeStaticData.VitalBonuses,"Vital bonus per point:", _vitalsDb);
      }
      EditorStatic.DrawThinLine(10);
      if (_attributesDb.Count() > 0)
      {
        EditorStatic.DrawBonusWidget(attributeStaticData.StatisticBonuses, "Statistic bonus per point:", _statisticsDb);
      }

   
      EditorStatic.DrawThinLine(10);
    }
    }

}
