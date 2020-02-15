using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Station
{
    [CreateAssetMenu]
    public class DefaultMechanics : StationMechanics
    {
        private DefaultFactionHandler _factionHandler;
     

        public override AsyncOperationHandle<GameObject>? OnCreateCharacter(PlayerCharacterType typeHandler, object[] data, Action<GameObject> onPlayerInstanced, string PrefabId)
        {
            _factionHandler = new DefaultFactionHandler();
            string raceId = (string) data[0];
            string secondId = (string) data[1];
            string genderId = (string) data[2];
            
            var dbSystem = _station.GetSystem<DbSystem>();
            var raceDb = dbSystem.GetDb<RaceDb>();
   
            if (typeHandler.GetType() == typeof(PlayerCharacterType))
            {
                if (data.Length != 3)
                {
                    Debug.Log("need update");
                }

                if (string.IsNullOrEmpty(PrefabId))
                {
                    Debug.Log("MISSING CHARACTER address");
               
                }
                else
                {
                    var op = Addressables.InstantiateAsync(PrefabId);
                    return op;
                }
            }

            return null;
        }

        public override void OnBuildPlayer(BaseCharacter character, PlayersData save, PlayerClassModel classData)
        {
            var dbSystem = _station.GetSystem<DbSystem>();
            var classDb = dbSystem.GetDb<PlayerClassDb>();
            var ActiveAbilityDb = dbSystem.GetDb<ActiveAbilitiesDb>();
            var PassiveAbilityDb = dbSystem.GetDb<PassiveAbilitiesDb>();
            var model = classDb.GetEntry(save.ClassId);
            if (model.StatsCalculator)
            {
                var calculatorInstance = Instantiate(model.StatsCalculator) as PlayerCalculations;
                calculatorInstance.PreSetup(classData);
                            
                character.Init(save.RaceId, save.FactionId, save.GenderId, calculatorInstance, save.Name);
                character.SetupAction(model.Attack);     
                character.AddMeta("classId", save.ClassId);
                character.AddMeta("icon", model.Icon);
                character.gameObject.name = "[player] "+save.Name;
                character.SetupStats(model.HealthVital,null,model.EnergyVitals.ToArray());
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

        public override void OnBuildNpc(BaseCharacter character, NpcModel model, string npcId)
        {
            if (model.StatsCalculator == null)
            {
                Debug.LogError("MISSING CHARACTER CALCULATOR");
                return;
            }

            var calculatorInstance = Instantiate(model.StatsCalculator) as NpcCalculation;
            if (calculatorInstance == null)
            {
                Debug.LogError("MISSING CHARACTER CALCULATOR INSTANCE");
                return;
            }

            calculatorInstance.PreSetup(model);
                            
            character.Init(model.RaceId, model.FactionId, "Male", calculatorInstance, model.Name);
            character.SetupAction(model.Attack);     
            character.AddMeta("npc_id", npcId);
            character.AddMeta("icon", model.Icon);
            character.gameObject.name = "[npc] "+model.Name;
            character.SetupStats(model.HealthVital,null,model.EnergyVitals.ToArray());
               
            #region ABILITIES
            //load from save
            List<RuntimeAbility> tempList = new List<RuntimeAbility>();
            foreach (var ab in model.OwnedAbilities)
            {
                var ability = new RuntimeAbility();
                ability.Initialize(ab,0 ,0, character, "");
                tempList.Add(ability);
            }
            character.Action.SetAbilities(tempList, character);
                
              
            //passive Abilities
            List<RuntimePassiveAbility> passiveList = new List<RuntimePassiveAbility>();
            /*foreach (var ab in save.LearnedPassiveAbilitiesList)
            {
                var ability = new RuntimePassiveAbility();
                ability.Initialize(PassiveAbilityDb.GetEntry(ab.Id),ab.Rename, character);
                passiveList.Add(ability);
            }
            */
                
            character.Action.SetPassiveAbilities(passiveList, character);

            //skills
                
                
            #endregion
        }

        public override void OnReceiveEvent(string eventName, object[] localParams)
        {
            
        }

        public override IFactionHandler FactionHandler()
        {
            return _factionHandler;
        }
        
        public override string Description()
        {
            return "this is the default mechanics used as demo";

        }
    }
}

