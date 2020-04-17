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
           var dbSystem = RpgStation.GetSystemStatic<DbSystem>();
            var classDb = dbSystem.GetDb<PlayerClassDb>();
            var ActiveAbilityDb = dbSystem.GetDb<ActiveAbilitiesDb>();
            var PassiveAbilityDb = dbSystem.GetDb<PassiveAbilitiesDb>();
            PlayerClassModel classData = (PlayerClassModel)data[0];
            PlayersData save = (PlayersData)data[1];
            var model = classDb.GetEntry(save.ClassId);
         
            if (model.StatsCalculator)
            {
                var calculatorInstance = Instantiate(model.StatsCalculator, character.transform) as PlayerCalculations;
                calculatorInstance.PreSetup(classData);
                
                character.Init(save.RaceId, save.FactionId, save.GenderId, calculatorInstance, save.Name);
                character.SetupAction(model.Attack);     
                character.AddMeta("classId", save.ClassId);
                character.AddMeta("icon", model.Icon);
                character.gameObject.name = "[player] "+save.Name;
                
                character.SetupStats(model.HealthVital,null,model.EnergyVitals.ToArray());
                character.Stats.SetVitalsValue(save.VitalStatus);
                character.GetInputHandler.InitializePlayerInput(PlayerInput.Instance);
                
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
                    ability.Initialize(PassiveAbilityDb.GetEntry(ab.Id),ab.Rename, character);
                    passiveList.Add(ability);
                }
                
                character.Action.SetPassiveAbilities(passiveList, character);

                //skills
                
                
                #endregion
            }
            else
            {
                Debug.LogError("MISSING CHARACTER CALCULATOR");
            }

        }
    }

}

