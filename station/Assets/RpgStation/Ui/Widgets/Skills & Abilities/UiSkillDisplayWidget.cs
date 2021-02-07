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

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _rank;
        #endregion
        public void Setup(BaseCharacter owner, RuntimeAbility ability)
        {
            if (_name)
            {
                _name.text = ability.Data.Name.GetValue();
            }
            if (_description)
            {
                _description.text = ability.Data.Description.GetValue();
            }
            if (_icon)
            {
                _icon.sprite = ability.Data.Icon;
            }

            if (_rank)
            {
                _rank.text = ability.Rank.ToString();
            }
        }
    }
}

