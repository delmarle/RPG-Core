using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    [Serializable]
    public class FootSoundTemplate : ScriptableObject
    {
        public List<SurfaceEntry> Entries = new List<SurfaceEntry>();
        public SurfaceEntry DefaultSurface;
        private Dictionary<string, SurfaceEntry> _map = new Dictionary<string, SurfaceEntry>();

        private void OnEnable()
        {
            _map.Clear();
            foreach (var entry in Entries)
            {
                _map.Add(entry.SurfaceName, entry);
            }
        }

        public SurfaceEntry ResolveSurface(string surfaceName)
        {
            if (_map.ContainsKey(surfaceName))
            {
                return _map[surfaceName];
            }

            return DefaultSurface;
        }
    }
}
