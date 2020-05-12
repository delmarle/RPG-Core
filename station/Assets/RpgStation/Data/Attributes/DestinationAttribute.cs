using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Station
{
    [AttributeUsage(AttributeTargets.Field), Serializable]
    public class DestinationAttribute : PropertyAttribute
    {
        [FormerlySerializedAs("scene")] [FormerlySerializedAs("Destination")] public DestinationModel destination;
    }
}
