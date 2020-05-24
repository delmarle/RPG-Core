using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiFeedWidget : UiNotificationElement
    {
        [SerializeField] private UiFeedEntry _objectPrefab;
        [SerializeField] private VerticalLayoutGroup _layout;
        private float _timeLeft;
        private readonly List<Dictionary<string, object>> _queue = new List<Dictionary<string, object>> ();
        private const int MaxConcurentEntries = 32;

        protected override void Start()
        {
            base.Start();
            PoolSystem.PopulatePool(_objectPrefab.gameObject, MaxConcurentEntries);
        }

        public override void ReceiveNotification(Dictionary<string, object> data)
        {
            _queue.Add(data);
        }
        
        private void Update()
        {
            if(_queue.Count>0 && _timeLeft <= 0)
            {
               
                if (_layout.transform.childCount > MaxConcurentEntries)
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
    }

}

