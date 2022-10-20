using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class SpawnerSave :  AreaSaveModule<Dictionary<string, SpawnerState>>
    {
        protected override void BuildDefaultData()
        {
            Value = new Dictionary<string, SpawnerState>();
        }

        public void AddEntry(string spawnerId, string entryId, EntityState state)
        {
            if (Value == null)
            {
                Value = new Dictionary<string, SpawnerState>();
            }

            if (Value.ContainsKey(spawnerId) == false)
            {
                Value.Add(spawnerId, new SpawnerState());
            }

            if (Value[spawnerId].SpawnsStateMap == null)
            {
                Value[spawnerId].SpawnsStateMap = new Dictionary<string, EntityState>();
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

        public SpawnerState GetDataById(string id)
        {
            if (Value != null && Value.ContainsKey(id))
            {
                return Value[id];
            }

            return null;
        }
    }

    [Serializable]
    public class SpawnerState
    {
        //string: ID of the entry in the spawner
        //string: json state
            //npc: position, rotation, vitals
            //prefab: position, rotation, exist
            //container: position, rotation, content
        public Dictionary<string, EntityState> SpawnsStateMap = new Dictionary<string, EntityState>();
    }

    [Serializable]
    public class EntityState
    {
        //SHARED
        public string EntityId;
        public SpawnObjectType EntityType; //NPC, ITEM, PREFAB, CONTAINER
        public Vector3 Position;
        public Vector3 Rotation;
        
        //NPC
        public List<IdIntegerValue> VitalStatus;
        
        //Container
        public ContainerState Container;
        public List<IdIntegerValue> Currencies;
    }

    public interface IEntityState
    {
        EntityState GetState();
    }

    public class EntityReference
    {
        public SpawnObjectType EntityType;
        public BaseCharacter Character;
        public Transform Object;
    }

}

