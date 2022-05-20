
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiBarLinkWidget : MonoBehaviour
    {
        [SerializeField] private Image _icon = null;
        [SerializeField] private UnityEngine.UI.Button _button = null;
        [SerializeField] private Image _cooldownImage = null;
        private CharacterAction _bufferedAction;
        private Timer _coolDownTimer;
        private ActionHandler _handler; 

        //link to ability
        //link to inventory

        public void BindAction(CharacterAction ability, ActionHandler handler)
        {
            _handler = handler;
            _bufferedAction = ability;

            if (ability == null)
            {
                _icon.enabled = false;
            }
            else
            {
                _icon.enabled = true;
                _icon.sprite = ability.GetIcon();
                _bufferedAction.OnStart += OnCoolDownStart;
                _bufferedAction.OnComplete += OnCoolDownStop;
            }
        }

        public void UseLink()
        {
            if (_bufferedAction != null && _handler != null)
            {
                if (_bufferedAction is RuntimeAbility ability)
                {
                    _handler.TryUseAction(ability);
                }
            }
        }

        public void OnCoolDownStart()
        {
            
            _button.interactable = false;
            float timeLeft = _bufferedAction.CdTimeLeft();
            _cooldownImage.fillAmount = 1;
            _coolDownTimer?.Cancel();
            _coolDownTimer = Timer.Register(timeLeft, () => { _cooldownImage.fillAmount = 0; }, f => _cooldownImage.fillAmount = f/timeLeft);
        }

        public void OnCoolDownStop()
        {

_button.interactable = true;
_coolDownTimer.Cancel();
_cooldownImage.fillAmount = 0;
 
        }
    }
}

