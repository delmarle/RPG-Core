using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public class SkillEditor
    {
        private static SkillDb _db;
        private static RaceDb _raceDb;
        private static PlayerClassDb _playerClassDb;
        private static StatisticDb _statisticDb;
        private static AttributesDb _attributesDb;
        private static VitalsDb _vitalsDb;
        private static ActiveAbilitiesDb _activeAbilitiesDb;
        
        private static Vector2 _propertyScrollPos;
        private static int _selectedIndexIndex = 0;
        private static List<bool> _levelsState = new List<bool>();
        private static GUIContent[] _requirementToolbarOptions;
        private static GUILayoutOption _requirementToolbarHeight;
        private static int _toolBarIndex;
        private static Vector2 _scrollPos;
        private static Texture2D _selectedTexture;
        private static bool _pickingTexture;

        private static bool CacheDbs()
        {
          bool missing = false;
          if (_db == null)
          {
            _db = (SkillDb)EditorStatic.GetDb(typeof(SkillDb));
            missing = true;
          }
          if (_raceDb == null)
          {
            _raceDb = (RaceDb)EditorStatic.GetDb(typeof(RaceDb));
            missing = true;
          }
          if (_playerClassDb == null)
          {
            _playerClassDb = (PlayerClassDb)EditorStatic.GetDb(typeof(PlayerClassDb));
            missing = true;
          }
          if (_statisticDb == null)
          {
            _statisticDb = (StatisticDb)EditorStatic.GetDb(typeof(StatisticDb));
            missing = true;
          }
          if (_attributesDb == null)
          {
            _attributesDb = (AttributesDb)EditorStatic.GetDb(typeof(AttributesDb));
            missing = true;
          }
          if (_vitalsDb == null)
          {
            _vitalsDb = (VitalsDb)EditorStatic.GetDb(typeof(VitalsDb));
            missing = true;
          }
          if (_activeAbilitiesDb == null)
          {
            _activeAbilitiesDb = (ActiveAbilitiesDb)EditorStatic.GetDb(typeof(ActiveAbilitiesDb));
            missing = true;
          }

          
          
          return missing;
        }

        public static void DrawContent()
        {
          if (CacheDbs() == true)
          {
            return;
          }

          EditorGUI.BeginChangeCheck();
          {
            DrawSkillList();
            ListView(_selectedIndexIndex);
          }
          if (EditorGUI.EndChangeCheck())
          {
            _db.ForceRefresh();
          }
        }
    
        #region [[ DRAW ]]

        private static void DrawSkillList()
        {
            _selectedIndexIndex = EditorStatic.DrawGenericSelectionList(_db, _selectedIndexIndex, _propertyScrollPos,out _propertyScrollPos,"user",false);
        }
        #endregion
        
         private static void Initialize()
    {
      LoadToolBar();
    }

    private static void ListView(int selectedSkill)
    {
      if(_requirementToolbarOptions == null) { Initialize(); }
      GUILayout.BeginHorizontal("box");
      {
        _scrollPos =
          EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        {
          SkillPanel(selectedSkill);
        }
        EditorGUILayout.EndScrollView();
      }
      GUILayout.EndHorizontal();
    }

    private static void SkillPanel(int selectedSkill)
    {
      var skillCount = _db.Count();
      if (selectedSkill == -1) return;
      if (skillCount == 0) return;
      if (skillCount < selectedSkill + 1) selectedSkill = 0;

      var staticData = _db.GetEntry(selectedSkill);
      GUILayout.Label("EDIT SKILL:", GUILayout.Width(70));
      EditorStatic.DrawLargeLine(5);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
        if (staticData.Icon)
          _selectedTexture = staticData.Icon.texture;
        else
          _selectedTexture = null;
        if (GUILayout.Button(_selectedTexture, GUILayout.Width(EditorStatic.EDITOR_BUTTON_SIZE),
          GUILayout.Height(EditorStatic.EDITOR_BUTTON_SIZE)))
        {
          int controllerId = GUIUtility.GetControlID(FocusType.Passive);
          EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, null, controllerId);
          _pickingTexture = true;
        }

        string commandName = Event.current.commandName;
        if (_pickingTexture && commandName == EditorStatic.EDITOR_OBJECT_PICKER_COMMAND_NAME)
        {
          _db.GetEntry(selectedSkill).Icon = EditorGUIUtility.GetObjectPickerObject() as Sprite;
          _pickingTexture = false;
        }

        GUILayout.BeginVertical();
        {
          GUILayout.Space(5);
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Name", GUILayout.Width(70));
            staticData.Name = GUILayout.TextField(staticData.Name);
          }
          GUILayout.EndHorizontal();
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Description", GUILayout.Width(70));
            staticData.Description = GUILayout.TextArea(staticData.Description, GUILayout.Height(45));
          }
          GUILayout.EndHorizontal();
          GUILayout.Space(3);

        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete skill?",
            "Do you want to delete: " + staticData.Name, "Delete", "Cancel"))
          {
            _db.Remove(staticData);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
      EditorStatic.DrawLargeLine();
      DrawLearningRequirement(staticData.LearningRequirement);

      EditorStatic.DrawLargeLine();
      DrawLevels(staticData);
     
    }

    #region [[ REQUIREMENT & RESTRICTIONS ]]
    private static void DrawLearningRequirement(Requirement requirement)
    {
      EditorStatic.DrawSectionTitle("Learning Requirements: ", 300, 5);
      _toolBarIndex = GUILayout.Toolbar(_toolBarIndex, _requirementToolbarOptions, EditorStatic.ToolBarStyle, _requirementToolbarHeight);
      switch (_toolBarIndex)
      {
          case 0:
            DrawRaceRestriction(requirement);
            break;
          case 1:
            DrawClassRestriction(requirement);
            break;
          case 2:
            DrawAttributeRequirement(requirement);
            break;
          case 3:
            DrawVitalRequirement(requirement);
            break;
          case 4:
            DrawStatRequirement(requirement);
            break;
          case 5:
            DrawSkillRequirement(requirement);
            break;
      }
    }

    private static void DrawRaceRestriction(Requirement requirement)
    {
      bool raceEnabled = requirement.RaceRestriction != null;
      if (raceEnabled)
      {
        if (EditorStatic.Button(true, 32, "Disable Race "+requirement.RaceRestriction.Mode, "bullet_green"))
        {
          requirement.RaceRestriction = null;
        }

      }
      else
      {
        if (EditorStatic.Button(true, 32, "Enable Race Requirement", "bullet_black"))
        {
          requirement.RaceRestriction = new IntegerRestriction();
        }
      }

      if (requirement.RaceRestriction != null)
      {
        requirement.RaceRestriction.Mode =
          (IntegerRestriction.RestrictionMode) EditorGUILayout.EnumPopup("Mode: ", requirement.RaceRestriction.Mode);

        for (var index = 0; index < requirement.RaceRestriction.Saved.Count; index++)
        {
          var race = requirement.RaceRestriction.Saved[index];
          EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
          requirement.RaceRestriction.Saved[index] =
            EditorGUILayout.Popup("Race " + index, race, _raceDb.ListEntryNames());
          if (EditorStatic.SizeableButton(220, 18, "DELETE", ""))
          {
            requirement.RaceRestriction.Saved.RemoveAt(index);
          }

          EditorGUILayout.EndHorizontal();
        }

        if (EditorStatic.Button(true, 32, "Add Race", "plus"))
        {
          requirement.RaceRestriction.Saved.Add(0);
        }
      }
    }

    private static void DrawClassRestriction(Requirement requirement)
    {
      string restrictionName = "Class";
      bool classEnabled = requirement.ClassesRestriction != null;
      
      if (classEnabled)
      {
        if (EditorStatic.Button(true, 32, "Disable "+restrictionName+requirement.ClassesRestriction.Mode, "bullet_green"))
        {
          requirement.ClassesRestriction = null;
        }

      }
      else
      {
        if (EditorStatic.Button(true, 32, "Enable "+restrictionName, "bullet_black"))
        {
          requirement.ClassesRestriction = new IntegerRestriction();
        }
      }

      if (requirement.ClassesRestriction != null)
      {
        requirement.ClassesRestriction.Mode =
          (IntegerRestriction.RestrictionMode) EditorGUILayout.EnumPopup("Mode: ", requirement.ClassesRestriction.Mode);

        for (var index = 0; index < requirement.ClassesRestriction.Saved.Count; index++)
        {
          var entry = requirement.ClassesRestriction.Saved[index];
          EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
          requirement.ClassesRestriction.Saved[index] =
            EditorGUILayout.Popup(restrictionName + index, entry, _playerClassDb.ListEntryNames());
          if (EditorStatic.SizeableButton(220, 18, "DELETE", ""))
          {
            requirement.ClassesRestriction.Saved.RemoveAt(index);
          }

          EditorGUILayout.EndHorizontal();
        }

        if (EditorStatic.Button(true, 32, "Add "+restrictionName, "plus"))
        {
          requirement.ClassesRestriction.Saved.Add(0);
        }
      }
    }
    
    private static void DrawAttributeRequirement(Requirement requirement)
    {
      string requirementName = "attribute";
      bool isactive = requirement.AttributeRequirement != null;
      if (isactive)
      {
        if (EditorStatic.Button(true, 32, "Disable "+requirementName+" requirement", "bullet_green"))
        {
          requirement.AttributeRequirement = null;
        }

      }
      else
      {
        if (EditorStatic.Button(true, 32, "Enable "+requirementName+" requirement", "bullet_black"))
        {
          requirement.AttributeRequirement = new List<IdIntegerValue>();
        }
      }

      if (requirement.AttributeRequirement != null)
      {
        for (var index = 0; index < requirement.AttributeRequirement.Count; index++)
        {
          var entry = requirement.AttributeRequirement[index];
          EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
          int requirementIndex = _attributesDb.GetIndex(entry.Id);
          requirementIndex = EditorGUILayout.Popup(requirementName + index, requirementIndex, _attributesDb.ListEntryNames());
          requirement.AttributeRequirement[index].Id = _attributesDb.GetKey(requirementIndex);
          requirement.AttributeRequirement[index].Value = EditorGUILayout.IntField("Minimum value", entry.Value);
          
          if (EditorStatic.SizeableButton(120, 18, "DELETE", ""))
          {
            requirement.AttributeRequirement.RemoveAt(index);
          }

          EditorGUILayout.EndHorizontal();
        }

        if (EditorStatic.Button(true, 32, "Add "+requirementName, "plus"))
        {
          requirement.AttributeRequirement.Add(new IdIntegerValue(_attributesDb.GetKey(0),5));
        }
      }
    }
    
    private static void DrawVitalRequirement(Requirement requirement)
    {
      string requirementName = "vital";
      bool isactive = requirement.VitalRequirement != null;
      if (isactive)
      {
        if (EditorStatic.Button(true, 32, "Disable "+requirementName+" requirement", "bullet_green"))
        {
          requirement.VitalRequirement = null;
        }

      }
      else
      {
        if (EditorStatic.Button(true, 32, "Enable "+requirementName+" requirement", "bullet_black"))
        {
          requirement.VitalRequirement = new List<IdIntegerValue>();
        }
      }

      if (requirement.VitalRequirement != null)
      {
        for (var index = 0; index < requirement.VitalRequirement.Count; index++)
        {
          var entry = requirement.VitalRequirement[index];
          var requirementIndex = _vitalsDb.GetIndex(entry.Id);
          EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
          requirementIndex = EditorGUILayout.Popup(requirementName + index, requirementIndex, _vitalsDb.ListEntryNames());
          requirement.VitalRequirement[index].Id = _vitalsDb.GetKey(requirementIndex);
          requirement.VitalRequirement[index].Value = EditorGUILayout.IntField("Minimum value", entry.Value);
          
          if (EditorStatic.SizeableButton(120, 18, "DELETE", ""))
          {
            requirement.VitalRequirement.RemoveAt(index);
          }

          EditorGUILayout.EndHorizontal();
        }

        if (EditorStatic.Button(true, 32, "Add "+requirementName, "plus"))
        {
          requirement.VitalRequirement.Add(new IdIntegerValue(_vitalsDb.GetKey(0),5));
        }
      }
    }
    
    private static void DrawStatRequirement(Requirement requirement)
    {
      string requirementName = "Stat";
      bool isactive = requirement.StatRequirement != null;
      if (isactive)
      {
        if (EditorStatic.Button(true, 32, "Disable "+requirementName+" requirement", "bullet_green"))
        {
          requirement.StatRequirement = null;
        }
      }
      else
      {
        if (EditorStatic.Button(true, 32, "Enable "+requirementName+" requirement", "bullet_black"))
        {
          requirement.StatRequirement = new List<IdFloatValue>();
        }
      }

      if (requirement.StatRequirement != null)
      {
        for (var index = 0; index < requirement.StatRequirement.Count; index++)
        {
          var entry = requirement.StatRequirement[index];
          EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
          var requirementIndex = _statisticDb.GetIndex(entry.Id);
          requirementIndex = EditorGUILayout.Popup(requirementName + index, requirementIndex, _statisticDb.ListEntryNames());

          requirement.StatRequirement[index].Id = _statisticDb.GetKey(requirementIndex);
          requirement.StatRequirement[index].Value = EditorGUILayout.FloatField("Minimum value", entry.Value);
          
          if (EditorStatic.SizeableButton(120, 18, "DELETE", ""))
          {
            requirement.StatRequirement.RemoveAt(index);
          }

          EditorGUILayout.EndHorizontal();
        }

        if (EditorStatic.Button(true, 32, "Add "+requirementName, "plus"))
        {
          requirement.StatRequirement.Add(new IdFloatValue(_statisticDb.GetKey(0),5));
        }
      }
    }
    
    private static void DrawSkillRequirement(Requirement requirement)
    {
      string required = "Skill";
      bool isactive = requirement.SkillRequirement != null;
      if (isactive)
      {
        if (EditorStatic.Button(true, 32, "Disable "+required+" requirement", "bullet_green"))
        {
          requirement.SkillRequirement = null;
        }

      }
      else
      {
        if (EditorStatic.Button(true, 32, "Enable "+required+" requirement", "bullet_black"))
        {
          requirement.SkillRequirement = new List<IdIntegerValue>();
        }
      }

      if (requirement.SkillRequirement != null)
      {
        if (EditorStatic.Button(true, 32, "Add "+required, "plus"))
        {
          requirement.SkillRequirement.Add(new IdIntegerValue(_db.GetKey(0),0));
        }
        for (var index = 0; index < requirement.SkillRequirement.Count; index++)
        {
          var entry = requirement.SkillRequirement[index];
          EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
          var requirementIndex = _db.GetIndex(entry.Id);
          requirementIndex =  EditorGUILayout.Popup(required + index, requirementIndex, _db.ListEntryNames());
          requirement.SkillRequirement[index].Id = _db.GetKey(requirementIndex);
          requirement.SkillRequirement[index].Value = EditorGUILayout.IntField("Minimum level", entry.Value);
          
          if (EditorStatic.SizeableButton(120, 18, "DELETE", ""))
          {
            requirement.SkillRequirement.RemoveAt(index);
          }

          EditorGUILayout.EndHorizontal();
        }
      }
    }
    #endregion
    
    #region [[ LEVELS ]]

    private static void DrawLevels(SkillData data)
    {
      while (_levelsState.Count < data.Levels.Count)
      {
        _levelsState.Add(false);
      }
      
      EditorStatic.DrawSectionTitle(string.Format("Levels: ({0})", data.Levels.Count), 300, 5);
      if (EditorStatic.Button(true, 32, "Add Level", "plus"))
      {
        data.Levels.Add(new SkillLevel());
        return;
      }
     
      for (int index = 0; index < data.Levels.Count; index++)
      {
        EditorGUILayout.Space();
        var level = data.Levels[index];
        EditorGUILayout.BeginHorizontal();
        _levelsState[index] = LevelFoldout("Level " + (index+1), _levelsState[index]);
        if (EditorStatic.SizeableButton(20, 16, "X", ""))
        {
          data.Levels.RemoveAt(index);
          return;
        }

        EditorGUILayout.EndHorizontal();
        if (_levelsState[index])
        {
          level.PointToNext = EditorGUILayout.IntField("Point required ", level.PointToNext);
          EditorStatic.DrawBonusWidget(level.AttributesBonuses,"Attributes Bonus:", _attributesDb);
          EditorStatic.DrawBonusWidget(level.StatisticBonuses,"Statistic Bonus:", _statisticDb);
          EditorStatic.DrawBonusWidget(level.VitalBonuses,"Vitals Bonus:", _vitalsDb);
          EditorStatic.DrawThinLine();
          
          if (EditorStatic.Button(true, 32, "Add active ability granted", "plus"))
          {
            level.AbilitiesGranted.Add(0);
          }
          for (var abilityIndex = 0; abilityIndex < level.AbilitiesGranted.Count; abilityIndex++)
          {
            var entry = level.AbilitiesGranted[abilityIndex];
            EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
            level.AbilitiesGranted[abilityIndex] = EditorGUILayout.Popup("Ability "+abilityIndex, entry, _activeAbilitiesDb.ListEntryNames());
           
            if (EditorStatic.SizeableButton(120, 18, "DELETE", ""))
            {
              level.AbilitiesGranted.RemoveAt(abilityIndex);
            }

            EditorGUILayout.EndHorizontal();
          }
        }
      }
    }
    
    #endregion

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
    private static void LoadToolBar()
    {
      _requirementToolbarHeight = GUILayout.Height(32);
      _requirementToolbarOptions = new GUIContent[6];
      
      _requirementToolbarOptions[0] = new GUIContent("<size=11><b> Race</b></size>",EditorStatic.GetEditorTexture("bullet_purple"),"");
      _requirementToolbarOptions[1] = new GUIContent("<size=11><b> Class</b></size>", EditorStatic.GetEditorTexture("user"), "");
      _requirementToolbarOptions[2] = new GUIContent("<size=11><b> Attribute</b></size>", EditorStatic.GetEditorTexture("bullet_yellow"), "");
      _requirementToolbarOptions[3] = new GUIContent("<size=11><b> Vital</b></size>", EditorStatic.GetEditorTexture("bullet_red"), "");
      _requirementToolbarOptions[4] = new GUIContent("<size=11><b> Stat</b></size>", EditorStatic.GetEditorTexture("bullet_green"), "");
      _requirementToolbarOptions[5] = new GUIContent("<size=11><b> Skills</b></size>", EditorStatic.GetEditorTexture("tree_list"), "");
      
    }
    }
}