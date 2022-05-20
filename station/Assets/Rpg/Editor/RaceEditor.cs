using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
  public static class RaceEditor
  {
    private static RaceDb _raceDb;
    private static VitalsDb _vitalsDb;
    private static AttributesDb _attributesDb;
    private static int _selectedEntryIndex = 0;
    private static Vector2 _propertyScrollPos;
    private static Vector2 _viewScrollPos;
    private static Vector2 _scrollPos;
    private static Texture2D _selectedTexture;
    private static bool _pickingTexture = false;
    private static bool _showAttributes = false;
    private static bool _showVitals = false;

    private static bool CacheDbs()
    {
      bool missing = false;
      if (_attributesDb == null)
      {
        _attributesDb = (AttributesDb) EditorStatic.GetDb(typeof(AttributesDb));
        missing = true;
      }

      if (_raceDb == null)
      {
        _raceDb = (RaceDb) EditorStatic.GetDb(typeof(RaceDb));
      }

      if (_vitalsDb == null)
      {
        _vitalsDb = (VitalsDb) EditorStatic.GetDb(typeof(VitalsDb));
        missing = true;
      }

      return missing;
    }

    public static void Draw()
    {
      CacheDbs();
      EditorGUI.BeginChangeCheck();
      {
        GUILayout.BeginHorizontal();
        DrawRaceList();
        ListView(_selectedEntryIndex);
        GUILayout.EndHorizontal();
      }
      if (EditorGUI.EndChangeCheck())
      {
        _raceDb.ForceRefresh(); 
      }
    }
    
    #region [[ DRAW ]]

    private static void DrawRaceList()
    {
      _selectedEntryIndex = EditorStatic.DrawGenericSelectionList(_raceDb, _selectedEntryIndex, _propertyScrollPos,out _propertyScrollPos,"user",false);
    }
    private static void ListView(int selectedRace)
    {
      var raceCount = _raceDb.Count();
      if (selectedRace == -1) return;
      if (raceCount == 0) return;
      if (raceCount < selectedRace+1) selectedRace = 0;
      
      var race = _raceDb.GetEntry(selectedRace);
      GUILayout.BeginHorizontal("box");
      {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
        {
          RacePanel(race,selectedRace);
        }
        EditorGUILayout.EndScrollView();
      }
      GUILayout.EndHorizontal();
    }

    private static void RacePanel(RaceModel raceStaticData,int selectedRace)
    {
      GUILayout.Label("EDIT RACE:",GUILayout.Width(70));
      EditorStatic.DrawLargeLine(5);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
        if (raceStaticData.Icon)
          _selectedTexture = raceStaticData.Icon.texture;
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
          _raceDb.GetEntry(selectedRace).Icon = EditorGUIUtility.GetObjectPickerObject() as Sprite;
          _pickingTexture = false;
        }
        
        GUILayout.BeginVertical();
        {
          GUILayout.Space(5);
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Name",GUILayout.Width(70));
            EditorStatic.DrawLocalization(raceStaticData.Name); 
          }
          GUILayout.EndHorizontal();
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Description",GUILayout.Width(70));
            EditorStatic.DrawLocalizationLabel(raceStaticData.Description, 45);
          }
          GUILayout.EndHorizontal();
          GUILayout.Space(3);
          
        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete race?",
            "Do you want to delete: "+raceStaticData.Name,"Delete","Cancel"))
          {
            _raceDb.Remove(raceStaticData);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
      EditorStatic.DrawThinLine(10);
      #region [[ BASE VALUES ]]

      
      #region[[ ATTRIBUTES STARTING VALUES]]
      _showAttributes = EditorStatic.LevelFoldout("Attributes starting values", _showAttributes, 28, Color.white);
      if (_showAttributes)
      {
        ForceSizeAttributeBonus(raceStaticData);
        int attributesIndex = 0;
        foreach (var currentBonus in raceStaticData.AttributeBonuses)
        {
          GUILayout.Space(3);
          currentBonus.Id = _attributesDb.GetKey(attributesIndex);
          string current = currentBonus.Id;
          var attribute = _attributesDb.GetEntry(current);
          currentBonus.Value = EditorGUILayout.IntField(attribute.Name+" ",currentBonus.Value);
          attributesIndex++;
        }
      }

      #endregion
      GUILayout.Space(3);
      #region[[ VITAL STARTING VALUES]]
      _showVitals = EditorStatic.LevelFoldout("Vital starting values", _showVitals, 28, Color.white);
      if (_showVitals)
      {
        ForceSizeVitalBonus(raceStaticData);
        int vitalIndex = 0;
        foreach (var currentBonus in raceStaticData.VitalBonuses)
        {
          GUILayout.Space(3);
          
          currentBonus.Id = _vitalsDb.GetKey(vitalIndex);
          string current = currentBonus.Id;
          var vital = _vitalsDb.GetEntry(current);
          currentBonus.Value = EditorGUILayout.IntField(vital.Name + " ", currentBonus.Value);
          vitalIndex++;
        }
      }

      #endregion
      #endregion
      
      EditorStatic.DrawThinLine(10);
    }
    
    private static void ForceSizeAttributeBonus(RaceModel raceStaticData)
    {
      int totalInDb = _attributesDb.Count();
      if(totalInDb == 0) raceStaticData.AttributeBonuses.Clear();
      
      while (raceStaticData.AttributeBonuses.Count < totalInDb)
      {
        
        var currentAttributeId = raceStaticData.AttributeBonuses.Count;
        var attributeKey = _attributesDb.GetKey(currentAttributeId);
        var baseValue = _attributesDb.GetEntry(currentAttributeId).DefaultValue;
        raceStaticData.AttributeBonuses.Add(new IdIntegerValue(attributeKey,baseValue));
      }
      while (raceStaticData.AttributeBonuses.Count > totalInDb) raceStaticData.AttributeBonuses.RemoveAt(raceStaticData.AttributeBonuses.Count-1);
    }

    private static void ForceSizeVitalBonus(RaceModel raceStaticData)
    {
      int totalInDb = _vitalsDb.Count();
      if(totalInDb == 0) raceStaticData.VitalBonuses.Clear();
      
      while (raceStaticData.VitalBonuses.Count < totalInDb)
      {
        var currentVitalId = raceStaticData.VitalBonuses.Count;
        var vitalKey = _vitalsDb.GetKey(currentVitalId);
        var baseValue = _vitalsDb.GetEntry(currentVitalId).DefaultValue;
        raceStaticData.VitalBonuses.Add(new IdIntegerValue(vitalKey,baseValue));
      }
      while (raceStaticData.VitalBonuses.Count > totalInDb) raceStaticData.VitalBonuses.RemoveAt(raceStaticData.VitalBonuses.Count-1);
    }
    #endregion
  }
}

