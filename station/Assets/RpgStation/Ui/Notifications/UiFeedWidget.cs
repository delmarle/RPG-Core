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
        private const int MaxConcurentEntries = 4;

        public override void ReceiveNotification(Dictionary<string, object> data)
        {
            _queue.Add(data);
            foreach (var val in data)
            {
                 Debug.Log($" {val.Key} - {val.Value}");
            }
        }
        
        private void Update()
        {
            if(_queue.Count>0)
            {
                if (_timeLeft <= 0 && _layout.transform.childCount <MaxConcurentEntries) 
                {
                    var entry = _queue[0];
                    _queue.RemoveAt (0);
                    var instance = GetNext();
                    instance.Setup (entry);
                    _timeLeft = 0.5f;
                }
            }
            _timeLeft -= Time.deltaTime;
        }

        private UiFeedEntry GetNext()
        {
            return PoolSystem.Spawn(_objectPrefab.gameObject,Vector3.zero,Quaternion.identity,_layout.transform).GetComponent<UiFeedEntry>();
        }
    }

}

