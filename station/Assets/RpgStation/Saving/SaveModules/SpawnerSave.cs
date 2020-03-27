using System;
using System.Collections.Generic;

namespace Station
{
    public class SpawnerSave :  SaveModule<Dictionary<string, SpawnerData>>
    {
        protected override void FetchData()
        {
            var sceneSystem = RpgStation.GetSystemStatic<SceneSystem>();
            if (Value == null)
            {
                Value = new Dictionary<string, SpawnerData>();
            }
        }

        public SpawnerData GetSpawnerDataById(string id)
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
        public Dictionary<string, string> SpawnsStateMap = new Dictionary<string, string>();
    }

}

