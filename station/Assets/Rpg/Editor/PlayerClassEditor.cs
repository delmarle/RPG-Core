using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class PlayerClassEditor
    {
        #region FIELDS
        private static RaceDb _raceDb;
        private static PlayerClassDb _classDb;
        private static VitalsDb _vitalsDb;
        private static AttributesDb _attributesDb;
        private static StatisticDb _statisticsDb;
        private static ActiveAbilitiesDb _activeAbilitiesDb;
        private static PassiveAbilitiesDb _passiveAbilitiesDb;
        private static SkillDb _SkillsDb;
        

        private static Texture2D _selectedTexture;
        private static bool _pickingTexture;
        private static int _selectedEntryIndex = 0;
        private static Vector2 _propertyScrollPos;
        private static Vector2 _viewScrollPos;
        private static bool _showRaces;
        private static bool _showStats;
        private static bool _showActiveAbilities;
        private static bool _showPassivesAbilities;
        private static bool _showSkills;
        private static bool _showAttack;
        #endregion

        private static bool CacheDbs()
        {
            bool missing = false;
            if (_classDb == null)
            {
                _classDb = (PlayerClassDb) EditorStatic.GetDb(typeof(PlayerClassDb));
                missing = true;
            }
            if (_raceDb == null)
            {
                _raceDb = (RaceDb) EditorStatic.GetDb(typeof(RaceDb));
                missing = true;
            }
            if (_vitalsDb == null)
            {
                _vitalsDb = (VitalsDb)EditorStatic.GetDb(typeof(VitalsDb));
                missing = true;
            }
            if (_attributesDb == null)
            {
                _attributesDb = (AttributesDb) EditorStatic.GetDb(typeof(AttributesDb));
                missing = true;
            }

            if (_statisticsDb == null)
            {
                _statisticsDb = (StatisticDb) EditorStatic.GetDb(typeof(StatisticDb));
                missing = true;
            }
            
            if (_activeAbilitiesDb == null)
            {
                _activeAbilitiesDb = (ActiveAbilitiesDb) EditorStatic.GetDb(typeof(ActiveAbilitiesDb));
                missing = true;
            }
            if (_passiveAbilitiesDb == null)
            {
                _passiveAbilitiesDb = (PassiveAbilitiesDb) EditorStatic.GetDb(typeof(PassiveAbilitiesDb));
                missing = true;
            }
            
            if (_SkillsDb == null)
            {
                _SkillsDb = (SkillDb) EditorStatic.GetDb(typeof(SkillDb));
                missing = true;
            }

            return missing;
        }
        
        public static void Draw()
        {
          
            EditorGUI.BeginChangeCheck();
            {
                if (CacheDbs() == false)
                {
                    EditorGUILayout.BeginHorizontal();
                    DrawList();
                    if (_selectedEntryIndex > _classDb.Count())
                    {
                        _selectedEntryIndex = 0;
                    }

                    if (_classDb.Any())
                    {
                        DrawView();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Add a Class.", MessageType.Warning);
                    }


                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.HelpBox("cannot find Db", MessageType.Warning);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                _classDb.ForceRefresh();
            }
        }

        private static void DrawList()
        {
            _selectedEntryIndex = EditorStatic.DrawGenericSelectionList(_classDb, _selectedEntryIndex,
                _propertyScrollPos, out _propertyScrollPos, "user", false);
        }

        private static void DrawView()
        {
            GUILayout.BeginHorizontal("box");
            {
                _viewScrollPos = EditorGUILayout.BeginScrollView(_viewScrollPos, GUILayout.ExpandHeight(true),
                    GUILayout.ExpandWidth(true));
                {
                    DrawViewContent();
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawViewContent()
        {
            var entry = _classDb.GetEntry(_selectedEntryIndex);
            if (entry == null)
            {

                EditorGUILayout.HelpBox("Select a Class", MessageType.Info);
                return;
            }
            EditorStatic.DrawLocalization(entry.Name, "Name: ");
            ClassPanel(entry, _selectedEntryIndex);
        }
        
        private static void ClassPanel(PlayerClassModel current, int selectedId)
        {
            GUILayout.Label("EDIT CLASS:", GUILayout.Width(90));
            EditorStatic.DrawLargeLine(5);
            FirstSection(current, selectedId);
            EditorStatic.DrawThinLine(10);
            RaceAllowedSection(current);
            EditorStatic.DrawThinLine(10);
            _showStats = EditorStatic.LevelFoldout("Statistics / Attributes / Vitals", _showStats, 28, Color.white);
            if (_showStats)
            {
                EditorGUILayout.HelpBox("BONUS ADDED TO SELECTED RACE STARTING VALUES", MessageType.Info);
                GUILayout.Space(5);
                current.StatsCalculator = (CharacterCalculation) EditorGUILayout.ObjectField("Calculator: ",
                    current.StatsCalculator, typeof(CharacterCalculation), false);
                RpgEditorStatic.DrawBonusWidget(current.AttributesBonuses, "Attribute Bonus:", _attributesDb);
                EditorStatic.DrawThinLine(10);
                HealthVitalSection(current);
                EditorStatic.DrawThinLine(10);
                VitalBonusSection(current);
                EditorStatic.DrawThinLine(10);
                RpgEditorStatic.DrawBonusWidget(current.StatisticsBonuses, "Statistic Bonus:", _statisticsDb);
            }

            EditorStatic.DrawThinLine(10);
            UpgradeClassSection(current);
            EditorStatic.DrawThinLine(10);
            _showSkills = EditorStatic.LevelFoldout("Owned Skills", _showSkills, 28, Color.white);
            if (_showSkills)
            {
                OwnedSkillsSection(current);
            }
            
            _showActiveAbilities = EditorStatic.LevelFoldout("Owned Active Abilities", _showActiveAbilities, 28, Color.white);
            if (_showActiveAbilities)
            {
                OwnedAbilitiesSection(current);
            }
            
            _showPassivesAbilities = EditorStatic.LevelFoldout("Owned Passive Abilities", _showPassivesAbilities, 28, Color.white);
            if (_showPassivesAbilities)
            {
                OwnedPassiveAbilitiesSection(current);
            }

            EditorStatic.DrawThinLine(10);
            _showAttack = EditorStatic.LevelFoldout("Attacking", _showAttack, 28, Color.white);
            if (_showAttack)
            {
                AttackSection(current);
            }
        }

        #region [[ SECTIONS ]] 

        private static void FirstSection(PlayerClassModel current, int selectedId)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                if (current.Icon)
                    _selectedTexture = current.Icon.texture;
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
                    _classDb.GetEntry(selectedId).Icon = EditorGUIUtility.GetObjectPickerObject() as Sprite;
                    EditorStatic.ResetFocus();
                    _pickingTexture = false;
                }

                GUILayout.BeginVertical();
                {
                    GUILayout.Space(5);
                    EditorStatic.DrawLocalization(current.Name, "Name :");
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Description",GUILayout.Width(70));
                        EditorStatic.DrawLocalizationLabel(current.Description, 45);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(3);
                }
                GUILayout.EndVertical();
                if (EditorStatic.ButtonDelete())
                {
                    if (EditorUtility.DisplayDialog("Delete class?",
                        "Do you want to delete: " + current.Name, "Delete", "Cancel"))
                    {
                        _classDb.Remove(current);
                        EditorStatic.ResetFocus();
                        return;
                    }
                }
            }
            GUILayout.EndHorizontal();
            current.FootSoundTemplate = (FootSoundTemplate) EditorGUILayout.ObjectField("Footsteps: ",
                current.FootSoundTemplate, typeof(FootSoundTemplate), false);
        }

        private static void RaceAllowedSection(PlayerClassModel current)
        {
            _showRaces = EditorStatic.LevelFoldout("Races allowed", _showRaces, 28, Color.white);
            if (_showRaces)
            {
                var raceNames = _raceDb.ListEntryNames();
                if (raceNames.Any())
                {
                    //draw list
                    int id = 0;
                    foreach (var entry in current.AllowedRaces)
                    {
                        #region [[ DRAW RACE ALLOWED

                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.BeginHorizontal(EditorStyles.centeredGreyMiniLabel);
                            {
                                GUILayout.BeginVertical();
                                {
                                    int raceIndex = _raceDb.GetIndex(entry.RaceId);
                                    raceIndex = EditorGUILayout.Popup(raceIndex, raceNames, GUILayout.Width(230));

                                    entry.RaceId = _raceDb.GetKey(raceIndex);
                                    entry.MaleAddressPrefab = EditorGUILayout.TextField("male prefab", entry.MaleAddressPrefab);
                                    entry.FemaleAddressPrefab = EditorGUILayout.TextField("female prefab", entry.FemaleAddressPrefab);
                                }
                                GUILayout.EndVertical();
                                if (EditorStatic.SizeableButton(65, 65, "DELETE", ""))
                                {
                                    current.AllowedRaces.RemoveAt(id);

                                    return;
                                }
                            }

                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                        id++;

                        #endregion
                    }
                    //add button

                    if (EditorStatic.SizeableButton(90, 30, "Add", "plus"))
                    {
                        var racesTotalInDb = _raceDb.Count();
                        if (racesTotalInDb <= current.AllowedRaces.Count)
                        {
                            return;
                        }

                        current.AllowedRaces.Add(new RaceMeta(_raceDb.GetKey(0)));
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No races.", MessageType.Warning);
                }
            }
        }
        
        private static void HealthVitalSection(PlayerClassModel current)
        {
            EditorStatic.DrawSectionTitle("Vitals used:", 350, 3);
            var vitalNames = _vitalsDb.ListEntryNames();

            current.UseHealth = EditorGUILayout.Toggle("Use Health:", current.UseHealth);

            if (current.UseHealth)
            {
                GUILayout.BeginHorizontal("box");
                {
                    int healthIndex = _vitalsDb.GetIndex(current.HealthVital.Id);
                    healthIndex = EditorGUILayout.Popup("Primary Health:", healthIndex, vitalNames, GUILayout.Width(250));
                    if (healthIndex < 0)
                    {
                        healthIndex = 0;
                    }

                    current.HealthVital.Id = _vitalsDb.GetKey(healthIndex);
                    current.HealthVital.Value = EditorGUILayout.IntField("Bonus: ", current.HealthVital.Value, GUILayout.Width(250));
                }
                GUILayout.EndHorizontal();
                current.UseSecondaryHealth =
                    EditorGUILayout.Toggle("Use secondary Health:", current.UseSecondaryHealth);
                if (current.UseSecondaryHealth)
                {
                    GUILayout.BeginHorizontal("box");
                    {
                        int health2Index = _vitalsDb.GetIndex(current.SecondaryHealthVital.Id);
                        if (health2Index < 0)
                        {
                            health2Index = 0;
                        }
                        health2Index =  EditorGUILayout.Popup("Secondary Health:", health2Index, vitalNames, GUILayout.Width(250));
                        current.SecondaryHealthVital.Id = _vitalsDb.GetKey(health2Index);
                        current.SecondaryHealthVital.Value = EditorGUILayout.IntField("Bonus: ", current.SecondaryHealthVital.Value, GUILayout.Width(250));
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }

        private static void VitalBonusSection(PlayerClassModel current)
        {
            EditorGUILayout.LabelField("Energy Vitals:");
            var vitalNames = _vitalsDb.ListEntryNames();
            //draw list
            for (var index = 0; index < current.EnergyVitals.Count; index++)
            {
                var vtlBonus = current.EnergyVitals[index];
                GUILayout.BeginHorizontal("box");
                {
                    EditorGUILayout.LabelField("Energy " + index + ":", GUILayout.Width(145));
                    int vitalIndex = _vitalsDb.GetIndex(vtlBonus.Id);
                    if (vitalIndex < 0)
                    {
                        vitalIndex = 0;
                    }
                    vitalIndex = EditorGUILayout.Popup(vitalIndex, vitalNames, GUILayout.Width(100));
                    vtlBonus.Id = _vitalsDb.GetKey(vitalIndex);
                    GUILayout.Space(5);
                    vtlBonus.Value = EditorGUILayout.IntField("Bonus: ", vtlBonus.Value, GUILayout.Width(250));
                    GUILayout.Space(5);

                    if (EditorStatic.SizeableButton(65, 16, "DELETE", ""))
                    {
                        current.EnergyVitals.Remove(vtlBonus);
                        return;
                    }
                }

                GUILayout.EndHorizontal();
            }
            //add button

            if (EditorStatic.SizeableButton(90, 30, "Add", "plus"))
            {
                string latestEnergy = _vitalsDb.GetKey(0);
                current.EnergyVitals.Add(new IdIntegerValue(latestEnergy, 5));
            }
        }

        private static void UpgradeClassSection(PlayerClassModel current)
        {
            current.CanUpgradeClass =
                EditorGUILayout.ToggleLeft("Can be upgraded to other classes", current.CanUpgradeClass);

            if (current.CanUpgradeClass)
            {
                var arrayKeyClass = _classDb.Db.Keys.ToArray();
                var classesNames = _classDb.ListEntryNames();
                //draw list
                List<string> classesList = new List<string>();
                foreach (var keyClass in current.UpgradeClasses)
                {
                    GUILayout.BeginHorizontal(EditorStyles.centeredGreyMiniLabel);
                    {
                        int selectedClass = Array.IndexOf(arrayKeyClass, keyClass);
                        selectedClass = EditorGUILayout.Popup(selectedClass, classesNames, GUILayout.Width(130));
                        classesList.Add(arrayKeyClass[selectedClass]);
                        GUILayout.Space(5);
                        //delete button
                        if (EditorStatic.SizeableButton(65, 16, "DELETE", ""))
                        {
                            current.UpgradeClasses.Remove(keyClass);
                            return;
                        }
                    }
                    current.UpgradeClasses = classesList;
                    GUILayout.EndHorizontal();
                }

                //add button
                if (EditorStatic.SizeableButton(90, 30, "Add", "plus"))
                {
                    var totalClassInDb = _classDb.Count();
                    if (totalClassInDb <= current.UpgradeClasses.Count)
                    {
                        return;
                    }

                    current.UpgradeClasses.Add(_classDb.Db.Keys.FirstOrDefault());
                }
            }
        }

        private static void OwnedAbilitiesSection(PlayerClassModel current)
        { 
            var totalClassInDb = _activeAbilitiesDb.Count();
          var abNames = _activeAbilitiesDb.ListEntryNames();
          if (totalClassInDb > 0)
          {
              //draw list
              for (var index = 0; index < current.OwnedAbilities.Count; index++)
              {
                  var ability = current.OwnedAbilities[index];
                  GUILayout.BeginHorizontal("box");
                  {
                      EditorGUILayout.LabelField("Ability " + index + ":", GUILayout.Width(145));
                      var currentIndex = _activeAbilitiesDb.GetIndex(ability.Id);
          
                      currentIndex = EditorGUILayout.Popup(currentIndex, abNames, GUILayout.Width(100));
                      var newKey = _activeAbilitiesDb.GetKey(currentIndex);
                      ability.Id = newKey;
              
                      GUILayout.Space(5);
                      ability.Rank = EditorGUILayout.IntField("Rank: ", ability.Rank, GUILayout.Width(250));
                      GUILayout.Space(5);
    
                      if (EditorStatic.SizeableButton(65, 16, "DELETE", ""))
                      {
                          current.OwnedAbilities.Remove(ability);
                          return;
                      }
                  }
                  GUILayout.EndHorizontal();
              }
              if (EditorStatic.SizeableButton(90, 30, "Add", "plus"))
              {
            
    
                  if (totalClassInDb <= current.OwnedAbilities.Count)
                  {
                      return;
                  }
    
                  current.OwnedAbilities.Add(new RankedTimeIdSave(_activeAbilitiesDb.GetKey(0),0,0));
              }

          }
          else
          {
              EditorGUILayout.HelpBox("no abilities found", MessageType.Info);
          }
        }
        
        private static void OwnedPassiveAbilitiesSection(PlayerClassModel current)
        {
            var totalEntriesInDb = _passiveAbilitiesDb.Count();
            var abNames = _passiveAbilitiesDb.ListEntryNames();
            if (totalEntriesInDb > 0)
            {
                for (var index = 0; index < current.OwnedAbilities.Count; index++)
                {
                    var ability = current.OwnedPassiveAbilities[index];
                    GUILayout.BeginHorizontal("box");
                    {
                        EditorGUILayout.LabelField("Passive Ability " + index + ":", GUILayout.Width(145));
                        var currentIndex = _passiveAbilitiesDb.GetIndex(ability.Id);
          
                        currentIndex = EditorGUILayout.Popup(currentIndex, abNames, GUILayout.Width(100));
                        var newKey = _passiveAbilitiesDb.GetKey(currentIndex);
                        ability.Id = newKey;
              
                        GUILayout.Space(5);
                        ability.Rank = EditorGUILayout.IntField("Rank: ", ability.Rank, GUILayout.Width(250));
                        GUILayout.Space(5);
    
                        if (EditorStatic.SizeableButton(65, 16, "DELETE", ""))
                        {
                            current.OwnedPassiveAbilities.Remove(ability);
                            return;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                if (EditorStatic.SizeableButton(90, 30, "Add", "plus"))
                {
               
                    if (totalEntriesInDb <= current.OwnedPassiveAbilities.Count)
                    {
                        return;
                    }
    
                    current.OwnedPassiveAbilities.Add(new RankedIdSave(_passiveAbilitiesDb.GetKey(0),0));
                }
            }
            else
            {
                EditorGUILayout.HelpBox("no passive abilities found", MessageType.Info);
            }
        }
        private static void OwnedSkillsSection(PlayerClassModel current)
        {
            var totalEntriesInDb = _SkillsDb.Count();
            var skillNames = _SkillsDb.ListEntryNames();
            if (totalEntriesInDb > 0)
            {
                for (var index = 0; index < current.OwnedSkills.Count; index++)
                {
                    var skill = current.OwnedSkills[index];
                    GUILayout.BeginHorizontal("box");
                    {
                        EditorGUILayout.LabelField("Skill " + index + ":", GUILayout.Width(145));
                        var currentIndex = _SkillsDb.GetIndex(skill.Id);
          
                        currentIndex = EditorGUILayout.Popup(currentIndex, skillNames, GUILayout.Width(100));
                        var newKey = _SkillsDb.GetKey(currentIndex);
                        skill.Id = newKey;
              
                        GUILayout.Space(5);
                        skill.Rank = EditorGUILayout.IntField("Rank: ", skill.Rank, GUILayout.Width(250));
                        GUILayout.Space(5);
    
                        if (EditorStatic.SizeableButton(65, 16, "DELETE", ""))
                        {
                            current.OwnedSkills.Remove(skill);
                            return;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                if (EditorStatic.SizeableButton(90, 30, "Add", "plus"))
                {
                    if (totalEntriesInDb <= current.OwnedSkills.Count)
                    {
                        return;
                    }
    
                    current.OwnedSkills.Add(new RankedIdSave(_SkillsDb.GetKey(0),0));
                }
            }
            else
            {
                EditorGUILayout.HelpBox("no skills found", MessageType.Info);
            }
        }

        private static void AttackSection(PlayerClassModel current)
        {
             AttackEditor.DrawAttack(current.Attack);
        }

        #endregion
    }
}