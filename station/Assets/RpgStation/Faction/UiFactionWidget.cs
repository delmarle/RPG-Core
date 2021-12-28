using System;
using System.Collections;
using System.Collections.Generic;
using Station;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiFactionWidget : MonoBehaviour
    {

        #region FIELDS

        private const string SELECTED_STATE = "focus";
        private const string NORMAL_STATE = "normal";

        [SerializeField] private StateAnimation _animation;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _icon;
    
        private FactionModel _cacheFactionModel;
        private Action<FactionModel> _onSelectAction;
        #endregion

        public void SetupFactionData(FactionModel factionModel, Action<FactionModel> onSelectAction)
        {
            _cacheFactionModel = factionModel;
            _onSelectAction = onSelectAction;
            if (_nameText)
            {
                _nameText.text = _cacheFactionModel.Name.GetValue();
            }

            if (_descriptionText)
            {
                _descriptionText.text = _cacheFactionModel.Description.GetValue();
            }

            if (_icon)
            {
                _icon.sprite = _cacheFactionModel.Icon;
            }
        }

        public void OnSelect()
        {
            _onSelectAction?.Invoke(_cacheFactionModel);
        }

        public FactionModel GetFactionModel()
        {
            return _cacheFactionModel;
        }

        public void SetSelectedState()
        {
            if (_animation)
            {
                _animation.PlayState(SELECTED_STATE);
            }
            
        }

        public void SetNormalState()
        {
            if (_animation)
            {
                _animation.PlayState(NORMAL_STATE);
            }
        }
    }
}

