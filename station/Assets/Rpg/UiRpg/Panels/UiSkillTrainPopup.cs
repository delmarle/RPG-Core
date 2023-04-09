using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class UiSkillTrainPopup : UiPopup
    {
        #region FIELDS

        public const string POPUP_ID = "skill_train_popup";
        private BaseCharacter _demander;

        #endregion
        
        public void SetData(BaseCharacter owner, BaseCharacter demander, TrainSkillInteraction trainInteraction)
        {
            _demander = demander;
            
        }

        private void TrainRankUp(RankProgression skillToTrain)
        {
            
            if (_demander.Skills.Skills.ContainsKey(skillToTrain.Id))
            {
                _demander.Skills.AddSkillRank(skillToTrain.Id);
            }
            else
            {
                _demander.Skills.AddSkill(skillToTrain);
            }
            
        }
    }
}