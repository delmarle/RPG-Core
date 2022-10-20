using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public abstract class BaseSceneSpawner : MonoBehaviour
    {
        protected Dictionary<string, BaseCharacter> _spawnedNpcs = new Dictionary<string, BaseCharacter>();
        protected Dictionary<string, Transform> _spawneObjects = new Dictionary<string, Transform>();
        public abstract void Init(StationMechanics stationMechanics);

        protected void AddCachedEntity(string id, EntityReference entity)
        {
            switch (entity.EntityType)
            {
                case SpawnObjectType.NPC:
                    _spawnedNpcs.Add(id, entity.Character);
                    break;
                case SpawnObjectType.ITEM:
                    break;
                case SpawnObjectType.PREFAB:
                    _spawneObjects.Add(id, entity.Object);
                    
                    break;
                case SpawnObjectType.CONTAINER:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}