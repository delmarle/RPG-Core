using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class ConditionGroup : MonoBehaviour
    {
        public List<SimpleCondition> Conditions = new List<SimpleCondition>();
        public List<ConditionCharacter> ConditionsCharacter = new List<ConditionCharacter>();

        public bool IsValid(CoreCharacter owner)
        {
            foreach (var condition in Conditions)
            {
                if (condition.EvaluateCondition() == false)
                    return false;
            }

            foreach (var condition in ConditionsCharacter)
            {
                if (condition.EvaluateCondition(owner) == false)
                    return false;
            }
            
            return true;
        }
    }

}
