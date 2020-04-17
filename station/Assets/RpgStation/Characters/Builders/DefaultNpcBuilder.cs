using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
    public class DefaultNpcBuilder : CharacterBuilder
    {
        public override Type GetMatchingType()
        {
            return typeof(NpcCharacterType);
        }
        
        public override void Build(BaseCharacter character, BaseCharacterData baseData, object[] data)
        {
            NpcModel model = (NpcModel)data[0];
            
            
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
                            
            character.Init(model.RaceId, model.FactionId, "Male", calculatorInstance, model.Name);
            character.SetupAction(model.Attack);     
            character.AddMeta("npc_id", baseData.Identifier);
            character.AddMeta("icon", model.Icon);
            character.gameObject.name = "[npc] "+model.Name;
            character.SetupStats(model.HealthVital,null,model.EnergyVitals.ToArray());
            character.Stats.SetVitalsFull();
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

