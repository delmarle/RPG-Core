using System;
using Malee;
using UnityEngine;

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

