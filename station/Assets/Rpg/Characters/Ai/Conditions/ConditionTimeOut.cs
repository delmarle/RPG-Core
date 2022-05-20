using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Station
{
    
    public class ConditionTimeOut : FSMCondition
    {
        [SerializeField] private float _minTime;
        [SerializeField] private float _maxTime;
        
        private DateTime _timeOut;
        public override void OnEnter()
        {
            float randomTime = Random.Range(_minTime, _maxTime);
            _timeOut = DateTime.Now.AddSeconds(randomTime);
        }

        public override bool EstimateCondition()
        {
            return _timeOut < DateTime.Now;
        }

        public override void OnExit()
        {
        
        }
    }
}

