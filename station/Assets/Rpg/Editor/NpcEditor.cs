using System.Linq;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public class 
        NpcEditor
    {
        #region FIELDS
        private static FactionDb _factionDb;
        private static RaceDb _raceDb;
        private static NpcDb _npcDb;
        private static VitalsDb _vitalsDb;
        private static AttributesDb _attributesDb;
        private static StatisticDb _statisticsDb;
        private static PassiveAbilitiesDb _passiveAbilitiesDb;
        private static SkillDb _SkillsDb;
        

        private static Texture2D _selectedTexture;
        private static bool _pickingTexture;
        private static int _selectedEntryIndex = 0;
        private static Vector2 _propertyScrollPos;
        private static Vector2 _viewScrollPos;
        private static bool _showStats;
 
        private static bool _showAbilitie;
        private static bool _showAttack;
        #endregion
        
          private static bool CacheDbs()
        {
            bool missing = false;
            if (_factionDb == null)
            {
                _factionDb = (FactionDb) EditorStatic.GetDb(typeof(FactionDb));
                missing = true;
            }

            if (_npcDb == null)
            {
                _npcDb = (NpcDb) EditorStatic.GetDb(typeof(NpcDb));
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
                    if (_selectedEntryIndex > _npcDb.Count())
                    {
                        _selectedEntryIndex = 0;
                    }

                    if (_npcDb.Any())
                    {
                        DrawView();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Add an NPC.", MessageType.Warning);
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
                _npcDb.ForceRefresh();
            }
        }

        private static void DrawList()
        {
            _selectedEntryIndex = EditorStatic.DrawGenericSelectionList(_npcDb, _selectedEntryIndex,
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
            var entry = _npcDb.GetEntry(_selectedEntryIndex);
            if (entry == null)
            {

                EditorGUILayout.HelpBox("Select an npc", MessageType.Info);
                return;
            }
            ClassPanel(entry, _selectedEntryIndex);
        }
        
        private static void ClassPanel(NpcModel current, int selectedId)
        {
            GUILayout.Label("EDIT CLASS:", GUILayout.Width(90));
            EditorStatic.DrawLargeLine(5);
            FirstSection(current, selectedId);
            current.PrefabId = EditorGUILayout.TextField("Prefab id: ", current.PrefabId);
            current.Brain = (CharacterBrain)EditorGUILayout.ObjectField("Brain Prefab: ", current.Brain, typeof(CharacterBrain), false);
            EditorStatic.DrawLargeLine(5);
            RaceSection(current);
            EditorStatic.DrawLargeLine(5);
            _showStats = EditorStatic.LevelFoldout("Statistics / Attributes / Vitals", _showStats, 28, Color.white);
            if (_showStats)
            {
                EditorGUILayout.HelpBox("BONUS ADDED TO SELECTED RACE STARTING VALUES", MessageType.Info);
                GUILayout.Space(5);
                current.StatsCalculator = (CharacterCalculation) EditorGUILayout.ObjectField("Calculator: ", current.StatsCalculator, typeof(CharacterCalculation), false);
                EditorStatic.DrawBonusWidget(current.AttributesBonuses, "Attribute Bonus:", _attributesDb);
                EditorStatic.DrawThinLine(5);
                HealthVitalSection(current);
                EditorStatic.DrawThinLine(5);
                VitalBonusSection(current);
                EditorStatic.DrawThinLine(5);
                EditorStatic.DrawBonusWidget(current.StatisticsBonuses, "Statistic Bonus:", _statisticsDb);
            }
            
            EditorStatic.DrawLargeLine(5);
            _showAbilitie = EditorStatic.LevelFoldout("Owned Abilities", _showAbilitie, 28, Color.white);
            if (_showAbilitie)
            {
                ExtraAbilitiesSection(current);
            }

            EditorStatic.DrawLargeLine(5);
            _showAttack = EditorStatic.LevelFoldout("Attacking", _showAttack, 28, Color.white);
            if (_showAttack)
            {
                AttackEditor.DrawAttack(current.Attack);
            }
            EditorStatic.DrawLargeLine(5);
            //TOGGLABLE
            
            current.LootTable = LootTableEditor.DrawExternalTableReference(current.LootTable, "Loot table:");
            EditorStatic.DrawLargeLine(5);
            current.InteractionLines =
                InteractionEditor.DrawInteractionLineList(current.InteractionLines, "Interactions:");
        }
        
         private static void FirstSection(NpcModel current, int selectedId)
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
                    _npcDb.GetEntry(selectedId).Icon = EditorGUIUtility.GetObjectPickerObject() as Sprite;
                    EditorStatic.ResetFocus();
                    _pickingTexture = false;
                }

                GUILayout.BeginVertical();
                {
                    GUILayout.Space(5);
                    current.Name = EditorGUILayout.TextField("Name ", current.Name);
                    current.Description = EditorStatic.DrawTextArea("Description: ", current.Description);

                    GUILayout.Space(3);
                }
                GUILayout.EndVertical();
                if (EditorStatic.ButtonDelete())
                {
                    if (EditorUtility.DisplayDialog("Delete class?",
                        "Do you want to delete: " + current.Name, "Delete", "Cancel"))
                    {
                        _npcDb.Remove(current);
                        EditorStatic.ResetFocus();
                        return;
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private static void RaceSection(NpcModel current)
        {
            var raceNames = _raceDb.ListEntryNames();
            if (raceNames.Any())
            {
                if (string.IsNullOrEmpty(current.RaceId))
                {
                    current.RaceId = _raceDb.GetKey(0);
                }

                int raceIndex = _raceDb.GetIndex(current.RaceId);
                raceIndex = EditorGUILayout.Popup("Race: ", raceIndex, raceNames, GUILayout.Width(300));
                    
                current.RaceId = _raceDb.GetKey(raceIndex);
            }
            else
            {
                EditorGUILayout.HelpBox("No races.", MessageType.Warning);
            }

            var factionNames = _factionDb.ListEntryNames();
            if (factionNames.Any())
            {
                if (string.IsNullOrEmpty(current.FactionId))
                {
                    current.FactionId = _factionDb.GetKey(0);
                }

                int factionIndex = _factionDb.GetIndex(current.FactionId);
                factionIndex = EditorGUILayout.Popup("Faction: ",factionIndex, factionNames, GUILayout.Width(300));
                    
                current.FactionId = _factionDb.GetKey(factionIndex);
            }
            else
            {
                EditorGUILayout.HelpBox("No factions.", MessageType.Warning);
            }
        }
        
        private static void HealthVitalSection(NpcModel current)
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
                        int healthIndex = _vitalsDb.GetIndex(current.SecondaryHealthVital.Id);
                        healthIndex = EditorGUILayout.Popup("Primary Health:", healthIndex, vitalNames, GUILayout.Width(250));
                        if (healthIndex < 0)
                        {
                            healthIndex = 0;
                        }

                        current.SecondaryHealthVital.Id = _vitalsDb.GetKey(healthIndex); 
                        current.SecondaryHealthVital.Value = EditorGUILayout.IntField("Bonus: ", current.SecondaryHealthVital.Value, GUILayout.Width(250));
                    }
                    GUILayout.EndHorizontal();
                }
                
            }
        }

        private static void VitalBonusSection(NpcModel current)
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
                    int energyIndex = _vitalsDb.GetIndex(vtlBonus.Id);
                    energyIndex = EditorGUILayout.Popup("Primary Health:", energyIndex, vitalNames, GUILayout.Width(250));
                    if (energyIndex < 0)
                    {
                        energyIndex = 0;
                    }

                    vtlBonus.Id =_vitalsDb.GetKey(energyIndex); 
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
                current.EnergyVitals.Add(new IdIntegerValue(_vitalsDb.GetKey(0), 5));
            }
        }
        
        private static void ExtraAbilitiesSection(NpcModel current)
        {
           
          
            //draw list
            for (var index = 0; index < current.OwnedAbilities.Count; index++)
            {
                var ability = current.OwnedAbilities[index];
                GUILayout.BeginVertical("box");
                {
                   // ActiveAbilityEditor.DrawActiveAbility(ability,false);
                }
                GUILayout.EndVertical();
            }
            if (EditorStatic.SizeableButton(90, 30, "Add", "plus"))
            {
                current.OwnedAbilities.Add(new ActiveAbility());
            }

        }
        
    }
}

