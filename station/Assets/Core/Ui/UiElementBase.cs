using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Station
{
    public class UiElementBase : PooledItem
    {
        #region Variables

        [Header("State On Start")] public bool ForceHide = true;
        public bool ForceResetPosition = true;

        [Header("Sounds ( Optional )")] public SoundConfig ShowSound;
        public SoundConfig HideSound;

        public bool IsVisible { get; protected set; }


        private RectTransform _rectTransform;

        protected RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        #endregion

        #region EVENTS

        [Header("Editor Events")] public UnityEvent OnHide;

        public UnityEvent OnShow;

        #endregion


        protected virtual void Awake()
        {
            OnLevelLoaded();

            if (ForceResetPosition)
            {
                RectTransform.anchoredPosition = Vector2.zero;
            }
        }

        protected virtual void Start()
        {
         
        }

       
        public void OnLevelLoaded(int level)
        {
            OnLevelLoaded();
        }

        protected virtual void OnLevelLoaded()
        {
            if (ForceHide)
            {
                HideFirst();
                InvokeHideEvent();
            }
            else
            {
                IsVisible = true;
            }
        }


        #region Invoke Events

        public void InvokeHideEvent()
        {
            if (OnHide != null)
                OnHide.Invoke();

            if (HideSound != null) EffectSoundPlayer.PlayEffectSound(HideSound.name);
        }

        public void InvokeShowEvent()
        {
            if (OnShow != null)
                OnShow.Invoke();

            if (ShowSound != null) EffectSoundPlayer.PlayEffectSound(ShowSound.name);
        }

        #endregion

        protected virtual void SetChildrenActive(bool active)
        {
            foreach (Transform trans in transform)
                trans.gameObject.SetActive(active);


            Image img = gameObject.GetComponent<Image>();
            if (img)
                img.enabled = active;
        }


        public virtual void Toggle()
        {
            if (IsVisible)
                Hide();
            else
                Show();
        }

        public virtual void Show()
        {
            Show(true);
        }

        public virtual void Show(bool triggerEvents)
        {
            if (IsVisible)
                return;

            IsVisible = true;
            SetChildrenActive(true);


            if (triggerEvents)
                InvokeShowEvent();
        }

        public void Show(float waitTime)
        {
            Show(waitTime, true);
        }

        public void Show(float waitTime, bool triggerEvents)
        {
            if (waitTime > 0.0f)
                StartCoroutine(_Show(waitTime, triggerEvents));
            else
                Show(triggerEvents);
        }

        protected virtual IEnumerator _Show(float waitTime, bool triggerEvents = true)
        {
            yield return StartCoroutine(WaitCorroutine(waitTime));
            Show(triggerEvents);
        }

        public virtual void HideFirst()
        {
            IsVisible = false;


            SetChildrenActive(false);
        }

        public virtual void Hide()
        {
            Hide(true);
        }

        public virtual void Hide(bool triggerEvents)
        {
            if (IsVisible == false)
                return;

            IsVisible = false;
            SetChildrenActive(false);

            if (triggerEvents)
                InvokeHideEvent();
        }

        public void Hide(float waitTime)
        {
            Hide(waitTime, true);
        }

        public void Hide(float waitTime, bool invokeEvents)
        {
            if (waitTime > 0)
                StartCoroutine(PrepareHide(waitTime, invokeEvents));
            else
                Hide(invokeEvents);
        }

        protected virtual IEnumerator PrepareHide(float waitTime, bool triggerEvents = true)
        {
            yield return StartCoroutine(WaitCorroutine(waitTime));
            Hide(triggerEvents);
        }

        protected IEnumerator WaitCorroutine(float waitTime)
        {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + waitTime)
            {
                yield return null;
            }
        }
    }
}
