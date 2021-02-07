using System;
using System.Collections;
using UnityEngine;

namespace Station
{
    
    public class UiElementAnim : UiElementBase
    {
        #region Variables

        [SerializeField] protected BaseAnimation _animation;
        [Header("Animation ( Optional )")] 
        [SerializeField] private string _showAnimation = "Open";
        [SerializeField] private string _hideAnimation = "Hide";



        private IEnumerator _animationCoroutine;
        


        #endregion

        private void PlayAnimation(string state, Action callback)
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);

            var duration = _animation? _animation.GetStateDuration(state):0;
            _animationCoroutine = AnimationCorroutine(duration + 0.01f, state, callback);
            StartCoroutine(_animationCoroutine);
        }


        public override void Show(bool triggerEvents)
        {
            if (IsVisible)
                return;
            
            IsVisible = true;
            SetChildrenActive(true);
        
            PlayAnimation(_showAnimation, null);


            if (triggerEvents)
                InvokeShowEvent();
        }


        public override void HideFirst()
        {
            IsVisible = false;

            SetChildrenActive(false);
        }


        public override void Hide(bool triggerEvents)
        {
            if (IsVisible == false)
                return;

            IsVisible = false;
            PlayAnimation(_hideAnimation, () =>
            {
                if (IsVisible == false)
                {
                    SetChildrenActive(false);
                }

            });


            if (triggerEvents)
                InvokeHideEvent();
        }


        protected virtual IEnumerator AnimationCorroutine(float waitTime, string state, Action callback)
        {
            yield return null;

            var before = _animationCoroutine;
        
            _animation?.PlayState(state);
            yield return StartCoroutine(WaitCorroutine(waitTime));

            callback?.Invoke();

            if (before == _animationCoroutine)
            {
                _animationCoroutine = null;
            }
        }

    }

}