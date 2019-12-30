using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class NpcSpawns : MonoBehaviour
    {
        public int SpawnAmount;
        public List<NpcSpawnData> Entries = new List<NpcSpawnData>();

        public NpcSpawnData GetDataById(string id)
        {
            foreach (var entry in Entries)
            {
                if (entry.Id == id) return entry;
            }

            return null;
        }
    }

    [Serializable]
    public class NpcSpawnData : IComparable<NpcSpawnData>
    {
        public string Id;
        public Vector3 position;
        public bool Unique;
        public int Weight;
        
        public int CompareTo(NpcSpawnData other)
        {
            return string.Compare(Id, other.Id, StringComparison.Ordinal);
        }
    }
}

