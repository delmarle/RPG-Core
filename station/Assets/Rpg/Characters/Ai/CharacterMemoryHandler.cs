using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class CharacterMemoryHandler : MonoBehaviour
    {
        #region FIELDS
        public delegate void CharacterTargetUpdated(BaseCharacter target);
        public CharacterTargetUpdated OnTargetChanged;
        private const float CYCLE_BEFORE_CLEAR = 15;
        [SerializeField] private float _fov = 120;
        [SerializeField] private float _visionRange = 30;
        [SerializeField] private AreaTracker _interactionTracker = null;
        [SerializeField] private LayerMask _raycastMask;
        
        private HashSet<BaseCharacter> _friendlyTargetsInRange = new HashSet<BaseCharacter>();
        private HashSet<BaseCharacter> _targetsInRange = new HashSet<BaseCharacter>();
        
        private Dictionary<BaseCharacter, HateState> _hateMap = new Dictionary<BaseCharacter, HateState>();
        private BaseCharacter _currentEnemy;
        public BaseCharacter GetCurrentEnemy => _currentEnemy;
        private List<BaseCharacter> _enemiesToForget = new List<BaseCharacter>();

        private ObjectPool<HateState> _statePool = new ObjectPool<HateState>(()=> new HateState(), 10);
        private BaseCharacter _owner;
        #endregion

        public void Activate(BaseCharacter owner)
        {
            _owner = owner;
            transform.SetParent(owner.transform);
            _interactionTracker.transform.localPosition = Vector3.zero;
            _interactionTracker.transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            _interactionTracker.Range = new Vector3(1*_visionRange, 0.5f *_visionRange, 1*_visionRange);
            _interactionTracker.AddOnNewDetected(OnDetect);
            _interactionTracker.AddOnNotDetected(OnUnDetect);
            AiSimulationSystem.AddSimulatedCharacter(this);
        }

        public void DesActivate()
        {
            _interactionTracker.RemoveOnNewDetected(OnDetect);
            _interactionTracker.RemoveOnNotDetected(OnUnDetect);
            AiSimulationSystem.RemoveSimulatedCharacter(this);
        }
        
        #region Detection
        private bool OnDetect(Tracker tracker, Target target)
        {
            var characterComponent = target.Character;
            if (characterComponent == null || characterComponent == _owner)
            {
                return false;
            }

            var stance = _owner.ResolveStance(characterComponent);
            if (stance == Stance.Ally)
            {
                _friendlyTargetsInRange.Add(characterComponent);
            }
            else
            {
                _targetsInRange.Add(characterComponent);
            }
            
            return true;
        }

        private void OnUnDetect(Tracker tracker, Target target)
        {
            var characterComponent = target.Character;
            if (characterComponent == null || characterComponent == _owner)
            {
                return;
            }
            
            var stance = _owner.ResolveStance(characterComponent);
            if (stance == Stance.Ally)
            {
                if (_friendlyTargetsInRange.Contains(characterComponent))
                {
                    _friendlyTargetsInRange.Add(characterComponent);
                }
            }
            else
            {
                if (_targetsInRange.Contains(characterComponent))
                {
                    _targetsInRange.Remove(characterComponent);
                }
            }
        }
        #endregion

        
        #region HATE CHANGES

        public void ReceiveHate(BaseCharacter source, int hateAmount)
        {
            var stance = _owner.ResolveStance(source);
            if (stance == Stance.Ally)
            {
                Debug.LogError($"trying to add hate for a friendly player");
            }
            else
            {
                if (_hateMap.ContainsKey(source))
                {
                    _hateMap[source].IncreaseHate(hateAmount);
                }
                else
                {
                    var data = _statePool.Create();
                    data.SetHate(hateAmount);
                    _hateMap.Add(source, data);
                }
            }
        }
        
        public void DecreaseHate(BaseCharacter source, int hateAmount)
        {
            var stance = _owner.ResolveStance(source);
            if (stance == Stance.Ally)
            {
                Debug.LogError($"trying to remove hate for a friendly player");
            }
            else
            {
                if (_hateMap.ContainsKey(source))
                {
                    _hateMap[source].IncreaseHate(hateAmount);
                }
                else
                {
                    var data = _statePool.Create();
                    _hateMap.Add(source, data);
                }
            }
        }

        public void ForgetTarget(BaseCharacter target)
        {
            if (_hateMap.ContainsKey(target))
            {
                var data = _hateMap[target];
                data.Reset();
                _statePool.Recycle(data);
                _hateMap.Remove(target);
            }

            if (_hateMap.Count == 0 &&  _currentEnemy != null)
            {
               
                ChangeTarget(null);
            }
        }

        #endregion

        public virtual void ProcessHate()
        {
            //if distance > 30
   
            //decide when seen
            foreach (var target in _targetsInRange)
            {
                if (_hateMap.ContainsKey(target) == false)
                {
                    //try to detect
                    if (DetectEnemy(target))
                    {
                        ReceiveHate(target, 10);
                    }
                }
            }

            BaseCharacter foundCharacter = null;
            int foundHate = 0;
            foreach (KeyValuePair<BaseCharacter, HateState> entry in _hateMap)
            {
                var target = entry.Key;
                if (target.IsDead)
                {
                    //remove
                    _enemiesToForget.Add(entry.Key);
                    continue;
                }

          
                entry.Value.IncreaseCycle();
                float distance = Vector3.Distance(_owner.GetFeet(), target.GetFeet());
                entry.Value.SetDistance(distance);
                if (distance > _visionRange )
                {
                    entry.Value.DecreaseHate(1);
                    if (entry.Value.CyclesSinceUpdate > CYCLE_BEFORE_CLEAR)
                    {
                        //out of combat
                        //decrease hate
                        Debug.Log($"decrease hate{entry.Value.GetHate}");
                        entry.Value.DecreaseHate(10);
                    }
                }
                
                if (entry.Value.GetHate >= foundHate)
                {
                    foundHate = entry.Value.GetHate;
                    foundCharacter = entry.Key;
                    
                }

                if (entry.Value.GetHate <= 0)
                {
                    _enemiesToForget.Add(entry.Key);
         
                }
            }

            ChangeTarget(foundCharacter);
            foreach (var delete in _enemiesToForget)
            {
                ForgetTarget(delete);
            }
            _enemiesToForget.Clear();
        }

        private bool DetectEnemy(BaseCharacter target)
        {

            if (RaycastUtils.IsVisible(_owner.transform, _owner.GetTop(), target.GetCenter(), _fov, _raycastMask))
            {
                return true;
            }
            
            return false;
        }

        private void ChangeTarget(BaseCharacter newTarget)
        {
            if (_currentEnemy == newTarget)
            {
                return;
            }
            
            _currentEnemy = newTarget;
            OnTargetChanged?.Invoke(newTarget);
        }
    }

    public class HateState
    {
        private int _hate;
        private int _cyclesSinceUpdate;
        public int GetHate => _hate + CalculateDistanceModified();
        public int CyclesSinceUpdate => _cyclesSinceUpdate;
        private float _distance;
        
        
        public void IncreaseHate(int amount)
        {
            _hate += amount;
        }

        public void DecreaseHate(int amount)
        {
            _hate -= amount;
        }

        public void SetDistance(float distance)
        {
            _distance = distance;
        }

        private int CalculateDistanceModified()
        {
            float modifier = 5;
            modifier *= _distance;
            return (int)Mathf.Clamp(modifier,0,5);
        }

        public void SetHate(int value)
        {
            _hate = value;
        }

        public void IncreaseCycle()
        {
            _cyclesSinceUpdate++;
        }

        public void Reset()
        {
            _hate = 0;
            _cyclesSinceUpdate = 0;

        }

    }
}


