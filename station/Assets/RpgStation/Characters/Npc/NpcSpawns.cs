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
    public class NpcSpawnData : SpawnData
    {
        
        
       
    }

    [Serializable]
    public class SpawnData : IComparable<SpawnData>
    {
        public SpawnObjectType SpawnType;
        public string Id;
        public GameObject Prefab;
        
        public Vector3 position;
        public bool Unique;
        public bool StateSaved;
        public int Weight;

        public string SaveId;
        
        public int CompareTo(SpawnData other)
        {
            return string.Compare(Id, other.Id, StringComparison.Ordinal);
        }
    }

    public enum SpawnObjectType
    {
        NPC, ITEM, PREFAB
    }

    //state saved bool, if true and spawned it will be always respawned, unless removed from save
    //different list: 
        //random, 
        //always on: will always spawn if condition is met and is not saved as removed if unique
        //state saved
    //spawn condition
    //respawn mode: none, timer, on enter scene
}

