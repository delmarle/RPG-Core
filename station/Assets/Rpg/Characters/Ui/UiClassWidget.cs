using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiClassWidget : MonoBehaviour
    {
        #region FIELDS

        private const string SELECTED_STATE = "focus";
        private const string NORMAL_STATE = "normal";

        [SerializeField] private StateAnimation _animation;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _icon;
    
        private PlayerClassModel _cacheModel;
        private Action<PlayerClassModel> _onSelectAction;
        #endregion

        public void SetupData(PlayerClassModel model, Action<PlayerClassModel> onSelectAction = null)
        {
            _cacheModel = model;
            _onSelectAction = onSelectAction;
            if (_nameText)
            {
                _nameText.text = _cacheModel.Name.GetValue();
            }

            if (_descriptionText)
            {
                _descriptionText.text = _cacheModel.Description.GetValue();
            }

            if (_icon)
            {
                _icon.sprite = _cacheModel.Icon;
            }
        }

        public void OnSelect()
        {
            _onSelectAction?.Invoke(_cacheModel);
        }

        public PlayerClassModel GetModel()
        {
            return _cacheModel;
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

