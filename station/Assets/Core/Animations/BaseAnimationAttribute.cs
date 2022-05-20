using System;
using UnityEngine;

namespace Station
{
    [AttributeUsage((AttributeTargets.Field)), Serializable]
    public class BaseAnimationAttribute : PropertyAttribute
    {
        public BaseAnimation Animation;
    }

}

