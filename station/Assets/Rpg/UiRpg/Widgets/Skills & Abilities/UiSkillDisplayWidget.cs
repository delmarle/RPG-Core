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
        
    }
}

