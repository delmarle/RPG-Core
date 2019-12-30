using System;
using UnityEngine;

namespace Station
{
    [AttributeUsage(AttributeTargets.Field), Serializable]
    public class DestinationAttribute : PropertyAttribute
    {
        public DestinationModel Destination;
    }
}
