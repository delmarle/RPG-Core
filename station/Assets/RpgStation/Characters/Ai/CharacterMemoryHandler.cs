using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class CharacterMemoryHandler : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private AreaTracker _interactionTracker = null;
        
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
            if (characterComponent == null)
            {
                return false;
            }

            var stance= _owner.ResolveStance(characterComponent);
            Debug.Log($"found character: {characterComponent} stance: {stance.ToString()}");
            //print("detect");
            return true;
        }

        private void OnUnDetect(Tracker tracker, Target target)
        {
            //print("detect");
        }
        #endregion

        
        #region HATE CHANGES

        public void ReceiveHate(BaseCharacter source, int hateAmount)
        {
            if (_hateMap.ContainsKey(source))
            {
               // _hateMap[source]
            }
            else
            {
                _hateMap.Add(source, 1);
            }
        }
        
        public void DecreaseHate(BaseCharacter source, int hateAmount)
        {
        }

        #endregion
        //scan for enemy
        //set target priority
        //list of targets
        //on target detected
        //on target forgoten
    }
}


