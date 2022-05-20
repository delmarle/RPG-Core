using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiFeedWidget : UiNotificationElement
    {
        private const float VISIBILITY_TIME = 10;
        private const float FADE_IN_TIME = 0.5F;
        private const float FADE_OUT_TIME = 2;
        private bool _visibleState;
        private float _timeVisible;
        
        [SerializeField] private UiFeedEntry _objectPrefab = null;
        [SerializeField] private VerticalLayoutGroup _layout = null;
        [SerializeField] private CanvasGroup _canvasGroup = null;
        private float _timeLeft;
        private readonly List<Dictionary<string, object>> _queue = new List<Dictionary<string, object>> ();
        private const int MaxConcurrentEntries = 32;

        protected override void Start()
        {
            base.Start();
            PoolSystem.PopulatePool(_objectPrefab.gameObject, MaxConcurrentEntries);
            _visibleState = false;
        }

        public override void ReceiveNotification(Dictionary<string, object> data)
        {
            _queue.Add(data);
            _visibleState = true;
            _timeVisible = VISIBILITY_TIME;
        }
        
        private void Update()
        {
            
            if(_queue.Count>0 && _timeLeft <= 0)
            {
               
                if (_layout.transform.childCount > MaxConcurrentEntries)
                {
                    DeSpawnLast();
                }
               
                
                var entry = _queue[0];
                _queue.RemoveAt (0);
                var instance = GetNext();
                instance.Setup (entry);
                _timeLeft = 0.1f;
            }
            _timeLeft -= Time.deltaTime;
            _timeVisible -= Time.deltaTime;
            HandleVisibility();
        }

        private UiFeedEntry GetNext()
        {
            return PoolSystem.Spawn(_objectPrefab.gameObject,Vector3.zero,Quaternion.identity,_layout.transform).GetComponent<UiFeedEntry>();
        }

        private void DeSpawnLast()
        {
            var lastChild = _layout.transform.GetChild(_layout.transform.childCount - 1);
            PoolSystem.Despawn(lastChild.gameObject);
        }
        
        #region VISIBILITY

        private void HandleVisibility()
        {
            if (_visibleState)
            {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.alpha = Mathf.Clamp(_canvasGroup.alpha + (Time.deltaTime/FADE_IN_TIME), 0, 1f);
            }
            else
            {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.alpha = Mathf.Clamp(_canvasGroup.alpha - (Time.deltaTime/FADE_OUT_TIME), 0, 1f);
            }

            _timeVisible -= Time.deltaTime;
            _visibleState = _timeVisible > 0;
        }

        #endregion
    }

}

