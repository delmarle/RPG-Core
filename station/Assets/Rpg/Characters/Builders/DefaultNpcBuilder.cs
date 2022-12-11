using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
    public class DefaultNpcBuilder : CharacterBuilder
    {
        public CharacterMemoryHandler MemoryHandlerPrefab;
        public override Type GetMatchingType()
        {
            return typeof(NpcCharacterType);
        }
        
        public override void Build(BaseCharacter character, BaseCharacterData baseData, object[] data)
        {
            NpcModel model = (NpcModel)data[0];
            var gameSettings = GameInstance.GetDb<GameSettingsDb>().Get();

            bool hasInteraction = model.InteractionLines?.Count > 0;
            if (hasInteraction)
            {
                var prefab = gameSettings.GetPrefab("entity_interaction");
                var instanceInteraction = Instantiate(prefab.Prefab, character.transform);
                var component = instanceInteraction.GetComponent<EntityInteraction>();
                instanceInteraction.transform.position = character.GetCenter();
                character.SetupInteraction(component,model.InteractionLines);
            }
            
            if (model.StatsCalculator == null)
            {
                Debug.LogError("MISSING CHARACTER CALCULATOR");
                return;
            }

            var calculatorInstance = Instantiate(model.StatsCalculator, character.transform) as NpcCalculation;
            if (calculatorInstance == null)
            {
                Debug.LogError("MISSING CHARACTER CALCULATOR INSTANCE");
                return;
            }

            calculatorInstance.PreSetup(model);
            var brainInstance = model.Brain? Instantiate(model.Brain, character.transform) : null;
            CharacterMemoryHandler instanceMemory = null;
            if (MemoryHandlerPrefab)
            {
                instanceMemory = Instantiate(MemoryHandlerPrefab);
            }

            character.Init(baseData.CharacterId, model.RaceId, model.FactionId, "Male", calculatorInstance,model.Name, brainInstance, instanceMemory);
            character.SetupAction(model.Attack);     
            character.AddMeta(RpgConst.NPC_KEY, baseData.Identifier);
            character.AddMeta(RpgConst.ICON_DATA, model.Icon);
            character.AddMeta(RpgConst.LOOT_TABLE_KEY, model.LootTable);
            character.gameObject.name = "[npc] "+model.Name;
            character.SetupStats(model.HealthVital,null,model.EnergyVitals.ToArray());
            character.Stats.SetVitalsFull();
            character.GetInputHandler.SetAiInput(null);
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
            /*
foreach (var ab in save.LearnedPassiveAbilitiesList)
{
    var ability = new RuntimePassiveAbility();
    ability.Initialize(PassiveAbilityDb.GetEntry(ab.Id),ab.Rename, character);
    passiveList.Add(ability);
}*/

    
character.Action.SetPassiveAbilities(passiveList, character);

//skills
    
    
#endregion
        }
} 

}

