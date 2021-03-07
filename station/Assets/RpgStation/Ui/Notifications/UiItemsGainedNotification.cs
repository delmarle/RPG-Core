using System;
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


        private const float DELAY_BETWEEN_EACH = 0.5f;
        private float _currentDelay = 0f;
        private List<ItemWidgetState> _spawnedMap = new List<ItemWidgetState>();
        #endregion

        protected override void Awake()
        {
            base.Awake();
          _localPool = new ObjectPool<UiItemWidget>(CreateInstance, 15);
          StartCoroutine(QueueRoutine());
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
        }

        IEnumerator QueueRoutine()
        {
            List<ItemWidgetState> entryToClear = new List<ItemWidgetState>();
            while (true)
            {
                entryToClear.Clear();
                if (_currentDelay > 0)
                {
                    _currentDelay -= Time.deltaTime;
                }
                else
                {
                    if (_queue.Count > 0)
                    {
                        var entry = _queue[0];
                        _queue.RemoveAt (0);
                        if (entry.ContainsKey(UiConstants.ORIGIN_POSITION))
                        {
                            var originPosition = (Vector3)entry[UiConstants.ORIGIN_POSITION];
                        }

                        if (entry.ContainsKey(UiConstants.ITEM_AMOUNT))
                        {
                            var itemId = (string)entry[UiConstants.ITEM_KEY];
                            var itemCount = (int)entry[UiConstants.ITEM_AMOUNT];
                            var instance = _localPool.Create();
                            instance.Setup(itemId, itemCount); 
                            instance.gameObject.SetActive(true);
                            _spawnedMap.Add(new ItemWidgetState(instance));
                        }
                        else if (entry.ContainsKey(UiConstants.ITEM_KEY))
                        {
                            var itemKey = (string)entry[UiConstants.ITEM_KEY];
                            var instance = _localPool.Create();
                            instance.Setup(itemKey);
                            instance.gameObject.SetActive(true);
                            _spawnedMap.Add(new ItemWidgetState(instance));
                        }
                        _currentDelay = DELAY_BETWEEN_EACH;
                    }

                    
                    for (var index = 0; index < _spawnedMap.Count; index++)
                    {
                        var entry = _spawnedMap[index];
                        switch (entry.CurrentState)
                        {
                            case ItemWidgetState.WidgetStateEnum.Spawned:
                                entry.DecreaseVisibleTime();
                                if (entry.VisibleTimeLeft <= 0)
                                {
                                    entry.ChangeStateToHide();
                                }

                                break;
                            case ItemWidgetState.WidgetStateEnum.Hiding:
                                entry.DecreaseDeSpawnTime();

                                if (entry.TimeLeftToDeSpawn <= 0)
                                {
                                    _localPool.Recycle(entry.Widget);
                                    entryToClear.Add(entry);
                                  
                                }

                                break;
                        }
                        
                        
                    }

                    foreach (var entry in entryToClear)
                    {
                        _spawnedMap.Remove(entry);
                        entry.Widget.gameObject.SetActive(false);
                    }
                }

                yield return null;
            }
            
        }

        private class ItemWidgetState
        {
            public UiItemWidget Widget;
            public float VisibleTimeLeft;
            public float TimeLeftToDeSpawn;

            public WidgetStateEnum CurrentState;

            public ItemWidgetState(UiItemWidget widget)
            {
                Widget = widget;
                VisibleTimeLeft = 3f;
                TimeLeftToDeSpawn = 0.5f;
                CurrentState = WidgetStateEnum.Spawned;
            }

            public void DecreaseVisibleTime()
            {
                VisibleTimeLeft -= Time.deltaTime;
            }

            public void ChangeStateToHide()
            {
                Widget.DoHide();
                CurrentState = WidgetStateEnum.Hiding;
            }
            
            public void DecreaseDeSpawnTime(){ TimeLeftToDeSpawn -= Time.deltaTime;}
            
            
            public enum WidgetStateEnum
            {
                Spawned,
                Hiding
            }
        }
    }
}

