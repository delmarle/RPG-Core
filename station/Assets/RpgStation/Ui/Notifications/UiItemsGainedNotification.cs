using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiItemsGainedNotification : UiNotificationElement
    {
        #region FIELDS

        [SerializeField] private UiItemWidget _prefab;
        [SerializeField] private LayoutGroup _layoutGroup;
        private readonly List<Dictionary<string, object>> _queue = new List<Dictionary<string, object>> ();
        private ObjectPool<UiItemWidget> _localPool;
        #endregion

        protected override void Awake()
        {
            base.Awake();
          _localPool = new ObjectPool<UiItemWidget>(CreateInstance, 15);
        }

        UiItemWidget CreateInstance()
        {
            var instance = Instantiate(_prefab, _layoutGroup.transform);
            instance.gameObject.SetActive(false);
            return instance;
            
        }

        public override void ReceiveNotification(Dictionary<string, object> data)
        {
            _queue.Add(data);
     
            
            if (data.ContainsKey(UiConstants.ORIGIN_POSITION))
            {
                var originPosition = (Vector3)data[UiConstants.ORIGIN_POSITION];
            }

            if (data.ContainsKey(UiConstants.ITEM_STACK))
            {
                var itemStack = (ItemStack)data[UiConstants.ITEM_STACK];
                var instance = _localPool.Create();
                instance.Setup(itemStack); 
                instance.gameObject.SetActive(true);
            }
            else if (data.ContainsKey(UiConstants.ITEM_KEY))
            {
                var itemKey = (string)data[UiConstants.ITEM_KEY];
                var instance = _localPool.Create();
                instance.Setup(itemKey);
                instance.gameObject.SetActive(true);
            }
            
        }
    }
}

