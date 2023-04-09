using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class SkillHandler
    {
        public Dictionary<string, RankProgression> Skills = new Dictionary<string, RankProgression>();
        private BaseCharacter _owner;
        
        public void Setup(BaseCharacter source, List<RankProgression> skillsToAdd)
        {
            if (skillsToAdd != null)
            {
                foreach (var entry in skillsToAdd)
                {
                    Skills.Add(entry.Id, entry);
                }
            }
        }

        public List<RankProgression> GenerateSave()
        {
            var save = new List<RankProgression>();
            foreach (var skillProgression in Skills)
            {
                save.Add(skillProgression.Value);
            }
            return save;
        }

        public void AddSkill(RankProgression skillToAdd)
        {
            if (Skills.ContainsKey(skillToAdd.Id) == false)
            {
                Skills.Add(skillToAdd.Id, skillToAdd);
                _owner.OnSkillGained?.Invoke(_owner, skillToAdd, 0);
            }
            else
            {
                Debug.LogWarning("skill already added");
            }
        }

        public void RemoveSkill(RankProgression skillToRemove)
        {
            if (Skills.ContainsKey(skillToRemove.Id))
            {
                Skills.Remove(skillToRemove.Id);
                _owner.OnSkillRemoved?.Invoke(_owner, skillToRemove, 0);
            }
            else
            {
                Debug.LogWarning("skill cant be removed");
            }
        }

        public void AddSkillProgression(string skillId, int progress)
        {
            if (Skills.ContainsKey(skillId))
            {
                Skills[skillId].Progression += progress;
            }
            else
            {
                Debug.LogWarning("skill missing for progression");
            }
        }
        
        public void AddSkillRank(string skillId)
        {
            if (Skills.ContainsKey(skillId))
            {
                Skills[skillId].Progression = 0;
                Skills[skillId].Rank ++;
                //TODO learned something new?
            }
            else
            {
                Debug.LogWarning("skill missing for rank increase");
            }
        }
    }
}

