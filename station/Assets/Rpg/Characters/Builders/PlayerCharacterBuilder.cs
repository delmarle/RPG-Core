using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
    public class PlayerCharacterBuilder : CharacterBuilder
    {
        

        public override Type GetMatchingType()
        {
            return typeof(PlayerCharacterType);
        }

        public override void Build(BaseCharacter character, BaseCharacterData baseData, object[] data)
        {
            var classDb = GameInstance.GetDb<PlayerClassDb>();
            var ActiveAbilityDb = GameInstance.GetDb<ActiveAbilitiesDb>();
            var PassiveAbilityDb = GameInstance.GetDb<PassiveAbilitiesDb>();
            PlayerClassModel classData = (PlayerClassModel)data[0];
            PlayersData save = (PlayersData)data[1];
            var classModel = classDb.GetEntry(save.ClassId);
         
            if (classModel.StatsCalculator)
            {
                var calculatorInstance = Instantiate(classModel.StatsCalculator, character.transform) as PlayerCalculations;
                if (calculatorInstance == null)
                {
                    Debug.LogError("missing calculator");
                    return;
                }

                calculatorInstance.PreSetup(classData);
                character.gameObject.AddComponent<EquipmentHandler>();
                character.Init(baseData.CharacterId,save.RaceId, save.FactionId, save.GenderId, calculatorInstance, save.Name, null, null);
                character.SetupAction(classModel.Attack);     
                character.AddMeta(StationConst.CLASS_ID, save.ClassId);
                character.AddMeta(StationConst.CHARACTER_ID, data[2]);
                character.AddMeta(StationConst.ICON_DATA, classModel.Icon);
                character.gameObject.name = "[player] "+save.Name;
                
                character.SetupStats(classModel.HealthVital,null,classModel.EnergyVitals.ToArray());
                character.Skills.Setup(character, save.LearnedSkillList);
                character.Stats.SetVitalsValue(save.VitalStatus);
                character.GetInputHandler.InitializePlayerInput(RpgInput.Instance);
                
                #region ABILITIES
                //load from save
                List<RuntimeAbility> tempList = new List<RuntimeAbility>();
                foreach (var ab in save.LearnedActiveAbilitiesList)
                {
                    var ability = new RuntimeAbility();
                    ability.Initialize(ActiveAbilityDb.GetEntry(ab.Id),ab.Rank ,ab.CoolDown, character,ab.Id);
                    tempList.Add(ability);
                }
                character.Action.SetAbilities(tempList, character);
                
               //set binds
               var binds = new Dictionary<string,List<BarSlotState>>();
               var mainBarBinds = new List<BarSlotState>();
               foreach (var barState in save.BarStates)
               {
                   if (barState.Id == "main")
                   {
                       mainBarBinds = barState.Slots;
                   }
               }
               binds.Add("main", mainBarBinds);
                character.Action.BuildBinds(binds);
                //passive Abilities
                List<RuntimePassiveAbility> passiveList = new List<RuntimePassiveAbility>();
                foreach (var ab in save.LearnedPassiveAbilitiesList)
                {
                    var ability = new RuntimePassiveAbility();
                    ability.Initialize(PassiveAbilityDb.GetEntry(ab.Id),ab.Rank, character);
                    passiveList.Add(ability);
                }
                
                character.Action.SetPassiveAbilities(passiveList, character);

                //skills
                
                
                #endregion
                
                #region FOOTSTEPS

                var footsteps = character.GetComponentInChildren<FootstepsBehaviour>();
                if (footsteps == null)
                {
                    var animator = character.gameObject.GetComponentInChildren<Animator>();
                    footsteps = animator.gameObject.AddComponent<FootstepsBehaviour>();
                }

                footsteps.Setup(classData.FootSoundTemplate);
                #endregion
            }
            else
            {
                Debug.LogError("MISSING CHARACTER CALCULATOR");
            }

        }
    }

}

