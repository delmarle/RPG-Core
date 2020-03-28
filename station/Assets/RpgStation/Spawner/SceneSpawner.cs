using System;
using System.Collections.Generic;
using Malee;
using UnityEngine;
using Weighted_Randomizer;
using Object = UnityEngine.Object;

namespace Station
{
    public class SceneSpawner : MonoBehaviour
    {
        public InitMode InitMode;
        public string SpawnId;
        public int SpawnAmount;
        public EntitiesSelectionMode entitiesSelectionMode;
        public ReSpawnMode ReSpawnMode;
        [Reorderable] public SpawnDataList DataList = new SpawnDataList();

        //cached
        private SavingSystem _savingSystem;
        
        public void Init()
        {
            if (InitMode == InitMode.SAVED)
            {
                _savingSystem = RpgStation.GetSystemStatic<SavingSystem>();
                var spawnerSave = _savingSystem.GetModule<SpawnerSave>();
                var spawnStateMap = spawnerSave.GetDataById(SpawnId)?.SpawnsStateMap;
                if (spawnStateMap == null || spawnStateMap.Count == 0)
                {
                    //init it
                    if (entitiesSelectionMode == EntitiesSelectionMode.EACH)
                    {
                        foreach (var spawnableData in DataList)
                        {
                            spawnableData.SpawnEntity();
                            spawnerSave.AddEntry(SpawnId, spawnableData.Id, "todo_data");
                        }
                    }
                    else if (entitiesSelectionMode == EntitiesSelectionMode.RANDOM_FROM_AMOUNT)
                    {
                    }

                    spawnerSave.Save();
                }
                else
                {
                
                    foreach (var spawnedEntries in spawnStateMap)
                    {
                    
                        // spawnedEntries.Value
                    }
                }
            }
            else if(InitMode == InitMode.ALWAYS)
            {
                if (entitiesSelectionMode == EntitiesSelectionMode.EACH)
                {
                    foreach (var spawnableData in DataList)
                    {
                        spawnableData.SpawnEntity();
                    }
                }
                else if (entitiesSelectionMode == EntitiesSelectionMode.RANDOM_FROM_AMOUNT)
                {
                    //selects
                    var randomizer = DataList.GetRandomizer();
                    for (int i = 0; i < SpawnAmount; i++)
                    {
                        var data = randomizer.NextWithReplacement();
                        data.SpawnEntity();
                    }
                }

            }

          

        }

        public SpawnData GetDataById(string id)
        {
            foreach (var entry in DataList)
            {
                if (entry.Id == id) return entry;
            }

            return null;
        }
    }

    [Serializable]
    public class SpawnDataList : ReorderableArray<SpawnData>
    {
        private StaticWeightedRandomizer<SpawnData> _weightedRandomizer = null;
        
        public  StaticWeightedRandomizer<SpawnData> GetRandomizer()
        {
            if (_weightedRandomizer == null)
            {
                _weightedRandomizer = new StaticWeightedRandomizer<SpawnData>();
                foreach (var data in ToArray())
                {
                    _weightedRandomizer.Add(data,data.Weight);
                }
            }

           
            return _weightedRandomizer;
        }
    }
    
   
    
    [Serializable]
    public class SpawnData : IComparable<SpawnData>
    {
        public string Id;
        public SpawnObjectType SpawnType;
        public GameObject Prefab;
        public PositionProvider Position;
        public bool Unique;
        public int Weight;
        public string SaveId;
        public int CompareTo(SpawnData other)
        {
            return string.Compare(Id, other.Id, StringComparison.Ordinal);
        }

        public GameObject SpawnEntity()
        {
            GameObject obj = null;
            switch (SpawnType)
            {
                case SpawnObjectType.NPC:
                    obj = null;
                    break;
                case SpawnObjectType.ITEM:
                    obj = null;
                    break;
                case SpawnObjectType.PREFAB:
                    obj = Object.Instantiate(Prefab);
                    break;
            }

            if (obj == null)
            {
                return null;
            }
Debug.Log("spawn obj:"+obj);
            Position.Generate();
            obj.transform.position = Position.GetPosition();
            obj.transform.Rotate(Position.GetRotation());
            return obj;
        }
    }

    public enum SpawnObjectType
    {
        NPC,
        ITEM,
        PREFAB
    }

    public enum InitMode
    {
        SAVED,
        ALWAYS
    }

    public enum EntitiesSelectionMode
    {
        EACH,
        RANDOM_FROM_AMOUNT
    }

    public enum ReSpawnMode
    {
        NONE,
        TIMER,
        REACH_POPULATION_THRESHOLD
    }
}

