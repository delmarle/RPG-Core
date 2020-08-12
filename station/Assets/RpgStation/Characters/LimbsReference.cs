using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class LimbsReference : MonoBehaviour
    {
        [SerializeField]private LimbEntry[] _entries;
        private Dictionary<string, Transform> _map = new Dictionary<string, Transform>();

        private void Awake()
        {
            foreach (var entry in _entries)
            {
                if (_map.ContainsKey(entry.Id) == false)
                {
                    _map.Add(entry.Id, entry.Reference);
                }
            
            }
        }

        public Transform GetLimb(string limbId)
        {
            return _map.ContainsKey(limbId) ? _map[limbId] : transform;
        }
    }

    [Serializable]
    public class LimbEntry
    {
        public string Id;
        public Transform Reference;
    }
}

