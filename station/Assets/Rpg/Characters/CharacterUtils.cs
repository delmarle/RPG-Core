using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public static class CharacterUtils
    {
        public const string MAIN_STATE = "main";
        
        public static PlayersData CreateCharacterSave(
            PlayerClassModel classModel, 
            string playerName, 
            string raceId,
            string classId, 
            string genderId,
            string factionId,
            string zoneId,
            Vector3 position)
        {
            int index = 0;
            
            BarStateSave mainBarState = new BarStateSave(MAIN_STATE);
            foreach (var ability in classModel.OwnedAbilities)
            {
                var slot = new BarSlotState(LinkType.Ability, index);
                mainBarState.Slots.Add(slot);
                index++;
            }
         
            var linkBar = new List<BarStateSave> {mainBarState};
            var defaultSkills = new List<RankProgression>();
            foreach (var skillToAdd in classModel.OwnedSkills)
            {
                var skillProgress = new RankProgression {Id = skillToAdd.Id};
                defaultSkills.Add(skillProgress);
            }
            var player = new PlayersData
            {
                Name = playerName,
                ClassId = classId,
                RaceId = raceId,
                GenderId = genderId,
                LastZoneId = zoneId,
                LastPosition = position,
                LastRotation = Vector3.zero,
                FactionId = factionId,
                LearnedActiveAbilitiesList = classModel.OwnedAbilities,
                LearnedPassiveAbilitiesList = classModel.OwnedPassiveAbilities,
                LearnedSkillList = defaultSkills,
                BarStates = linkBar,
                VitalStatus = new List<IdIntegerValue>()
            };
            //set vitals
            if (classModel.UseHealth)
            {
                var healthStatus = new IdIntegerValue(classModel.HealthVital.Id,-1);
                player.VitalStatus.Add(healthStatus);
            }

            foreach (var energyData in classModel.EnergyVitals)
            {
                var energyStatus = new IdIntegerValue(energyData.Id,-1);
                player.VitalStatus.Add(energyStatus);
            }

            return player;
        }
  
    }  
}

