using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class CounterSystem : BaseSystem
    {
        #region fields

        private SavingSystem _savingSystem;
        private CounterData _data = new CounterData();
        public CounterData GetData => _data;
        #endregion
        
        protected override void OnInit()
        {
            GameGlobalEvents.OnEnterGame.AddListener(OnEnterGame);
            _savingSystem = GameInstance.GetSystem<SavingSystem>();
        }


        protected override void OnDispose()
        {
            GameGlobalEvents.OnEnterGame.RemoveListener(OnEnterGame);
        }

        protected override void OnDataBaseReady()
        {
            
        }

        private void OnEnterGame()
        {
            _data =_savingSystem.GetModule<CounterSaveModules>().Value;
        }


        public void IncreaseCounter(string key, int value = 1)
        {
            if (_data.Counters.ContainsKey(key))
            {
                _data.Counters[key] += value;
            }
            else
            {
                _data.Counters.Add(key, value); 
            }
        }

        public int GetCounter(string key)
        {
            if (_data.Counters.ContainsKey(key))
            {
                return _data.Counters[key];
            }
            return 0;
        }
        
        public void IncreaseCounter(string key, string id, int value = 1)
        {
            if (_data.CountersWithIds.ContainsKey(key))
            {
                if (_data.CountersWithIds[key].ContainsKey(id))
                {
                    _data.CountersWithIds[key][id] += value;
                }
                else
                {
                    _data.CountersWithIds[key].Add(id, value);
                }
            }
            else
            {
                _data.CountersWithIds.Add(key, new Dictionary<string, int>());
                _data.CountersWithIds[key].Add(id, value);
            }
        }

        public int GetCounter(string key, string id)
        {
            if (_data.CountersWithIds.ContainsKey(key))
            {
                if (_data.CountersWithIds[key].ContainsKey(id))
                {
                    return _data.CountersWithIds[key][id];
                }
            }
            
            return 0;
        }

    }
}

