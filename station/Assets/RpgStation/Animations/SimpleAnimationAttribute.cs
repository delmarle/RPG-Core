using System;
using UnityEngine;

namespace Station
{
    [AttributeUsage((AttributeTargets.Field)), Serializable]
    public class SimpleAnimationAttribute : PropertyAttribute
    {
        public CoreAnimation Animation;
    }

}

