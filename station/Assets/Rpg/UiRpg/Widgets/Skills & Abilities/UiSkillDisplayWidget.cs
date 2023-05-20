using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiSkillDisplayWidget : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private SkillDb _db;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _rank;
        [SerializeField] private LocalizedText _rankLocalization;
        [SerializeField] private StateAnimation _animation;
        
        #endregion
        public void Setup(BaseCharacter owner, RankProgression skillProgress)
        {
            var skillMeta = _db.GetEntry(skillProgress.Id);
            if (_name)
            {
                _name.text = skillMeta.Name;
            }
            if (_description)
            {
                _description.text = skillMeta.Description;
            }
            if (_icon)
            {
                _icon.sprite = skillMeta.Icon;
            }

            if (_rank)
            {
                string rankLoc = string.Format(_rankLocalization.GetValue(), skillProgress.Rank+1);
                _rank.text = rankLoc;
            }
        }

        public void SetupAsTrain(BaseCharacter owner, TrainSkillData trainSkill)
        {
            var skillMeta = _db.GetEntry(trainSkill.SkillId);
            bool hasSkill = owner.Skills.Skills.ContainsKey(trainSkill.SkillId);
            int playerSkillRank = owner.Skills.GetSkillRank(trainSkill.SkillId);
            bool isNotLastRank = hasSkill && trainSkill.TrainableRank.Length > playerSkillRank;
            if (_name)
            {
                _name.text = skillMeta.Name;
            }

            if (_animation)
            {
                _animation.PlayState(hasSkill? "state_has_skill":"state_not_learned");
                _animation.PlayState(isNotLastRank? "state_not_last_rank":"state_last_rank");
            }
            //can be trained or not
            if (hasSkill)
            {
                //has skill
                //rank trainable
                
            }
            else
            {
                //not learned yet
            }
        }
    }
}

