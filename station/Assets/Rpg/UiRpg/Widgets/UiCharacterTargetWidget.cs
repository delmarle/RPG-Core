using UnityEngine;

namespace Station
{
    public class UiCharacterTargetWidget : MonoBehaviour
    {
        #region FIELDS
        
        private const string HIDDEN = "hidden";
        private const string SHOW = "show";
        private const string HIDE = "hide";
        [SerializeField] private ClipsAnimation _animation = null;
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
                if (_animation.CurrentState == HIDDEN || _animation.CurrentState == HIDE)
                {
                    _animation.PlayState(SHOW);
                }

                _portraitWidget.Setup(target, null);
            }
            else
            {
                if (_animation.CurrentState  == null || _animation.CurrentState == SHOW)
                {
                    _animation.PlayState(HIDE);
                }
                
                _portraitWidget.Setup(null, null);
            }

        }
    }
}

