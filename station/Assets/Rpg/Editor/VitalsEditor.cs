using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class VitalsEditor
    {
        private static Vector2 _scrollPos;
        private static Texture2D _selectedTexture;
        private static bool _pickingTexture;
        private static Vector2 _propertyScrollPos;
        private static int _selectedVitalIndex = 1;
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
                ListView(_selectedVitalIndex);
              }
              else
              {
                EditorGUILayout.HelpBox("DBs are missing", MessageType.Warning);
              }

              
            }
            if (EditorGUI.EndChangeCheck())
            {
              _vitalsDb.ForceRefresh(); 
            }
        }
    
        #region [[ DRAW ]]

        private static void DrawVitalsList()
        {
            _selectedVitalIndex = EditorStatic.DrawGenericSelectionList(_vitalsDb, _selectedVitalIndex, _propertyScrollPos,out _propertyScrollPos,"bullet_red",false);
        }
        
            
    private static void ListView(int selectedVital)
    {
      var vitalsCount = _vitalsDb.Count();
      if (selectedVital == -1) return;
      if (vitalsCount == 0) return;
      if (vitalsCount < selectedVital+1) selectedVital = 0;
      
      var vital = _vitalsDb.GetEntry(selectedVital);
      GUILayout.BeginHorizontal("box");
      {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
        {
          string vitalId = _vitalsDb.GetKey(selectedVital);
          VitalPanel(vital,vitalId);
        }
        EditorGUILayout.EndScrollView();
      }
      GUILayout.EndHorizontal();
    }  

    private static void VitalPanel(VitalModel vitalStaticData,string selectedVital)
    {
      vitalStaticData.Id = selectedVital;
      GUILayout.Label("EDIT Vital:",GUILayout.Width(70));
      EditorStatic.DrawLargeLine(5);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
        if (vitalStaticData.Icon)
          _selectedTexture = vitalStaticData.Icon.texture;
        else
          _selectedTexture = null;

        if (GUILayout.Button(_selectedTexture, GUILayout.Width(EditorStatic.EDITOR_BUTTON_SIZE),GUILayout.Height(EditorStatic.EDITOR_BUTTON_SIZE)))
        {
          int controllerId = GUIUtility.GetControlID(FocusType.Passive);
          EditorGUIUtility.ShowObjectPicker<Sprite>(null,false,null,controllerId);
          _pickingTexture = true;
        }
        string commandName = Event.current.commandName;
        if (_pickingTexture &&commandName == EditorStatic.EDITOR_OBJECT_PICKER_COMMAND_NAME)
        {
          _vitalsDb.GetEntry(selectedVital).Icon = EditorGUIUtility.GetObjectPickerObject() as Sprite;
          _pickingTexture = false;
        }
        
        GUILayout.BeginVertical();
        {
          GUILayout.Space(5);
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Name",GUILayout.Width(70));
            vitalStaticData.Name = GUILayout.TextField(vitalStaticData.Name);
          }
          GUILayout.EndHorizontal();
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Description",GUILayout.Width(70));
            vitalStaticData.Description = GUILayout.TextArea(vitalStaticData.Description,GUILayout.Height(45));
          }
          GUILayout.EndHorizontal();
          
        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete vital?",
            "Do you want to delete: "+vitalStaticData.Name,"Delete","Cancel"))
          {
            _vitalsDb.Remove(vitalStaticData);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
      EditorStatic.DrawThinLine(10);
      vitalStaticData.Color = EditorGUILayout.ColorField("Color ",vitalStaticData.Color,GUILayout.ExpandWidth(true));
      GUILayout.Space(3);
      vitalStaticData.DefaultValue = EditorGUILayout.IntField("Default value ",vitalStaticData.DefaultValue,GUILayout.ExpandWidth(true));
      GUILayout.Space(3);
      vitalStaticData.RegenCycle = EditorGUILayout.Slider("Regen Cycle ",vitalStaticData.RegenCycle,0,12);
      vitalStaticData.RegenMode = (VitalModel.RegenType)EditorGUILayout.EnumPopup("Regen mode",vitalStaticData.RegenMode,GUILayout.ExpandWidth(true));
      GUILayout.Space(3);
      vitalStaticData.UsageType = (VitalModel.VitalType)EditorGUILayout.EnumPopup("Type",vitalStaticData.UsageType,GUILayout.ExpandWidth(true));
      EditorStatic.DrawThinLine(10);
      EditorStatic.DrawBonusWidget(vitalStaticData.AttributesBonuses, "Regen bonus per stat point:", _attributesDb);
    }
        #endregion
    }
}

