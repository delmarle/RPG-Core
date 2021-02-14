using System;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class ActionCastBarWidget : MonoBehaviour
    {

        #region FIELDS
        private enum ActionState
        {
            None,
            Casting,
            Action
        }
        private const string START_CASTING_STATE = "start_casting";
        private const string CANCEL_CASTING_STATE = "cancel_casting";
        private const string FINISH_CASTING_STATE = "finish_casting";

        private const string START_ACTION_STATE = "start_action";
        private const string FINISH_ACTION_STATE = "finish_action";

        private float _sliderTime = 0;

        [SerializeField] private BaseAnimation _animation;
        [SerializeField] private Slider _bar;
        private BaseCharacter _character;
        private CharacterAction _castingAction;
        private CharacterAction _currentAction;
        private float _castingTime;
        private float _actionTime;
        private ActionState _state;
        #endregion

        public void FollowCharacter(BaseCharacter character)
        {
            UnFollowCharacter();
            _character = character;
            var actionHandler = _character?.Action;
            if (actionHandler != null)
            {
                actionHandler.OnStartCasting += OnStartCasting;
                actionHandler.OnCompleteCasting += OnCompleteCasting;
                actionHandler.OnCancelCasting += OnCancelCasting;
                actionHandler.OnStartAction += OnStartAction;
                actionHandler.OnFinishAction += OnFinishAction;
            }
        }

        public void UnFollowCharacter()
        {
            var actionHandler = _character?.Action;
            if (actionHandler != null)
            {
                actionHandler.OnStartCasting += OnStartCasting;
                actionHandler.OnCompleteCasting += OnCompleteCasting;
                actionHandler.OnCancelCasting += OnCancelCasting;
                actionHandler.OnStartAction += OnStartAction;
                actionHandler.OnFinishAction += OnFinishAction;
            }
        }
        
        
        #region EVENTS CALL BACKS
        private void OnStartCasting(CharacterAction action)
        {
            _castingAction = action;
            _currentAction = null;
            _animation.PlayState(START_CASTING_STATE);
            _castingTime = 0;
            _castingAction.CalculateActionLength();
            _state = ActionState.Casting;
        }
        
        private void OnCancelCasting(CharacterAction action)
        {
            _castingAction = null;
            _currentAction = null;
            _animation.PlayState(CANCEL_CASTING_STATE);
            _state = ActionState.None;
        }
        
        private void OnCompleteCasting(CharacterAction action)
        {
            _castingAction = null;
            _currentAction = action;
            _animation.PlayState(FINISH_CASTING_STATE);
            _state = ActionState.Action;
        }
        
        private void OnStartAction(CharacterAction action)
        {
            _castingAction = null;
            _currentAction = action;
            _animation.PlayState(START_ACTION_STATE);
            _state = ActionState.Action;
            _actionTime = action.CalculateActionLength();
        }

        private void OnFinishAction(CharacterAction action)
        {
            _castingAction = null;
            _currentAction = null;
            _animation.PlayState(FINISH_ACTION_STATE);
            _state = ActionState.None;
        }
        #endregion

        private void Update()
        {
            if (_state == ActionState.None) return;

            if (_state == ActionState.Casting)
            {
                _castingTime += Time.deltaTime;
                _bar.maxValue = _castingAction.CalculateActionLength();
                _bar.value = _castingTime;
                
            }
            else
            {
                if (_currentAction.CalculateActionLength() > 0)
                {
                    _actionTime -= Time.deltaTime;
                    _bar.maxValue = _currentAction.CalculateActionLength();
                    _bar.value = _actionTime;
                }
                else
                {
                    _bar.maxValue = 1;
                    _bar.value = 1;
                }
            }
        }
    }
}
