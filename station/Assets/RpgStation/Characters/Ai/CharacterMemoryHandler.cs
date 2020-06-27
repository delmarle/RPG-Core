using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class CharacterMemoryHandler : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private AreaTracker _interactionTracker = null;
        
        private List<BaseCharacter> _friendlyTargets = new List<BaseCharacter>();
        private Dictionary<BaseCharacter, int> _hateMap = new Dictionary<BaseCharacter, int>();
        private BaseCharacter _owner;
        #endregion

        public void Activate(BaseCharacter owner)
        {
            _owner = owner;
            transform.SetParent(owner.transform);
            _interactionTracker.transform.localPosition = Vector3.zero;
            _interactionTracker.transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            _interactionTracker.AddOnNewDetected(OnDetect);
            _interactionTracker.AddOnNotDetected(OnUnDetect);
        }

        public void DesActivate()
        {
            _interactionTracker.RemoveOnNewDetected(OnDetect);
            _interactionTracker.RemoveOnNotDetected(OnUnDetect);
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
            switch (stance)
            {
                case Stance.Ally:
                    if (_friendlyTargets.Contains(characterComponent) == false)
                    {
                        _friendlyTargets.Add(characterComponent);
                    }
                    break;
                case Stance.Neutral:
                    if (_hateMap.ContainsKey(characterComponent) == false)
                    {
                        _hateMap.Add(characterComponent, 0);
                    }

                    break;
                case Stance.Enemy:
                    if (_hateMap.ContainsKey(characterComponent) == false)
                    {
                        _hateMap.Add(characterComponent, 10);
                    }
                    break;
            }
            Debug.Log($"found character: {characterComponent} stance: {stance.ToString()}");
            return true;
        }

        private void OnUnDetect(Tracker tracker, Target target)
        {
            var characterComponent = target.Character;
            if (characterComponent == null || characterComponent == _owner)
            {
                return;
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
                    _hateMap[source] += hateAmount;
                }
                else
                {
                    _hateMap.Add(source, hateAmount);
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
                    _hateMap[source] -= hateAmount;
                }
                else
                {
                    _hateMap.Add(source, 0);
                }
            }
        }

        #endregion
        //scan for enemy
        //set target priority
        //list of targets
        //on target detected
        //on target forgoten
    }
}


