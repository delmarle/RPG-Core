
using TMPro;
using UnityEngine;

namespace Station
{
    public class UiCharacterTargetWidget : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private UiCharacterPortraitWidget _portraitWidget = null;
        //status effects
        private BaseCharacter _character;
        #endregion

        public void Setup(BaseCharacter character)
        {
            _character = character;
            if (_character != null)
            {
                _character.OnTargetChanged-= OnTargetChanged;
                _character.OnTargetChanged+= OnTargetChanged;
                OnTargetChanged(_character.Target);
            }
            else
            {
                OnTargetChanged(null);
            }
        }

        public void OnDestroy()
        {
            _character.OnTargetChanged-= OnTargetChanged;
        }

        private void OnTargetChanged(BaseCharacter target)
        {
            if (target != null)
            {
                _portraitWidget.Setup(target);
            }
            else
            {
                _portraitWidget.Setup(null);
            }

        }
    }
}

