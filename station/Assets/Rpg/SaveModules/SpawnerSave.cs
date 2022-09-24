using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class SpawnerSave :  AreaSaveModule<Dictionary<string, SpawnerData>>
    {
        protected override void BuildDefaultData()
        {
            Value = new Dictionary<string, SpawnerData>();
        }

        public void AddEntry(string spawnerId, string entryId, object state)
        {
            if (Value == null)
            {
                Value = new Dictionary<string, SpawnerData>();
            }

            if (Value.ContainsKey(spawnerId) == false)
            {
                Value.Add(spawnerId, new SpawnerData());
            }

            if (Value[spawnerId].SpawnsStateMap == null)
            {
                Value[spawnerId].SpawnsStateMap = new Dictionary<string, object>();
            }

            if (Value[spawnerId].SpawnsStateMap.ContainsKey(entryId) == false)
            {
                Value[spawnerId].SpawnsStateMap.Add(entryId, state);
            }
            else
            {
                Value[spawnerId].SpawnsStateMap[entryId] = state;
            }
        }

        public SpawnerData GetDataById(string id)
        {
            if (Value != null && Value.ContainsKey(id))
            {
                return Value[id];
            }

            return null;
        }
    }

    [Serializable]
    public class SpawnerData
    {
        //string: ID of the entry in the spawner
        //string: json state
            //npc: position, rotation, vitals
            //prefab: position, rotation, exist
            //container: position, rotation, content
        public Dictionary<string, object> SpawnsStateMap = new Dictionary<string, object>();
    }

}

