using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class SimpleCondition: ScriptableObject
    {
        
        public virtual bool EvaluateCondition()
        {
            return false;
        }

    }

    public class ConditionCharacter : ScriptableObject
    {
        public virtual bool EvaluateCondition(CoreCharacter character)
        {
            return false;
        }
    }
}

