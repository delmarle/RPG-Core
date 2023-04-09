using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
    public class InteractionLine :ScriptableObject
    {
        #region FIELDs
        //Condition
        [SerializeField] private List<SimpleCondition> _conditions;
        [SerializeField] private List<ConditionCharacter> _characterConditions;
        
        [SerializeField] private LocalizedText _lockedText;
        [SerializeField] private LocalizedText _unlockedText;
        
        #endregion

        public string GetLockedLocalization()
        {
            return _lockedText.GetValue();
        }
        
        public string GetUnLockedLocalization()
        {
            return _unlockedText.GetValue();
        }
        private bool CanTriggerBase()
        {
            foreach (var condition in _conditions)
            {
                if (condition.EvaluateCondition() == false)
                {
                    return false;
                }
            }

            return true;
        }
        public bool CanTrigger(BaseCharacter character)
        {
            if (CanTriggerBase() == false)
            {
                return false;
            }

            foreach (var condition in _characterConditions)
            {
                if (condition.EvaluateCondition(character) == false)
                {
                    return false;
                }
            }
            return true;
        }
        
        public virtual void Trigger(BaseCharacter owner, BaseCharacter demander){}
    }
}