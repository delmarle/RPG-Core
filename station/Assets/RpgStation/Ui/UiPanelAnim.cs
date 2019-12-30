using System;
using System.Collections;
using UnityEngine;

namespace Station
{

    [RequireComponent(typeof(Animator))]
    public class UiPanelAnim : UiPanelBase
    {
        #region Variables


        [Header("Animation ( Optional )")] [SerializeField]
        private AnimationClip _showAnimation = null;

        public int ShowAnimId { get; protected set; }

        [SerializeField] private AnimationClip _hideAnimation = null;
        public int HideAnimId { get; protected set; }



        private IEnumerator _animationCoroutine;

        private Animator _animator;

        public Animator Animator
        {
            get
            {
                if (_animator == null)
                    _animator = GetComponent<Animator>();

                return _animator;
            }
        }


        #endregion

        protected override void Awake()
        {
            if (_showAnimation)
                ShowAnimId = Animator.StringToHash(_showAnimation.name);

            if (_hideAnimation)
                HideAnimId = Animator.StringToHash(_hideAnimation.name);

            base.Awake();
        }

        private void PlayAnimation(AnimationClip clip, int hash, Action callback)
        {
            if (clip != null)
            {
                if (_animationCoroutine != null)
                    StopCoroutine(_animationCoroutine);

                _animationCoroutine = AnimationCorroutine(clip.length + 0.1f, hash, callback);
                StartCoroutine(_animationCoroutine);
            }
            else
            {
                Animator.enabled = false;
                if (callback != null)
                    callback();

            }
        }


        public override void Show(bool triggerEvents)
        {
            if (IsVisible)
                return;
            
            IsVisible = true;
            SetChildrenActive(true);
            PlayAnimation(_showAnimation, ShowAnimId, null);


            if (triggerEvents)
                InvokeShowEvent();
        }


        public override void HideFirst()
        {
            IsVisible = false;
            //Animator.enabled = false;

            SetChildrenActive(false);
        }


        public override void Hide(bool triggerEvents)
        {
            if (IsVisible == false)
                return;

            IsVisible = false;
            PlayAnimation(_hideAnimation, HideAnimId, () =>
            {
                if (IsVisible == false)
                {
                    SetChildrenActive(false);
                }

            });


            if (triggerEvents)
                InvokeHideEvent();
        }


        protected virtual IEnumerator AnimationCorroutine(float waitTime, int hash, Action callback)
        {
            yield return null;

            var before = _animationCoroutine;
            Animator.enabled = true;
            Animator.Play(hash);

            yield return StartCoroutine(WaitCorroutine(waitTime));

            Animator.enabled = false;
            if (callback != null)
                callback();

            if (before == _animationCoroutine)
            {
                _animationCoroutine = null;
            }
        }

    }

}