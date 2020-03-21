using System;
using System.Collections.Generic;

namespace Station
{
    public class SpawnerSave :  SaveModule<Dictionary<string, SpawnerData>>
    {

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

