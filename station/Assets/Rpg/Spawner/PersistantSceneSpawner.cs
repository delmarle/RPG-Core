using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class PersistantSceneSpawner : BaseSceneSpawner
    {
        public string SpawnId;
        public int SpawnAmount;
        public EntitiesSelectionMode entitiesSelectionMode;
        public ReSpawnMode ReSpawnMode;
        public SpawnDataList DataList = new SpawnDataList();

        //cached
        private SavingSystem _savingSystem;
        private StationMechanics _mechanics;


        public override void Init(StationMechanics stationMechanics)
        {
            _mechanics = stationMechanics;
            _savingSystem = GameInstance.GetSystem<SavingSystem>();
               
            SpawnerSave spawnerSave = _savingSystem.GetModule<SpawnerSave>();
            var spawnStateMap = spawnerSave.GetDataById(SpawnId)?.SpawnsStateMap;
            bool hasNotSpawned = spawnStateMap == null || spawnStateMap.Count == 0;
            if (hasNotSpawned)
            {
                //init it
                if (entitiesSelectionMode == EntitiesSelectionMode.EACH)
                {
                    foreach (var spawnableData in DataList.Data)
                    {
                        spawnableData.SpawnEntity(_mechanics, spawnableData.Id, liveEntity =>
                        {
                            AddCachedEntity(spawnableData.Id, liveEntity);
                            switch (liveEntity.EntityType)
                            {
                                case SpawnObjectType.NPC:
                                    break;
                                case SpawnObjectType.ITEM:
                                    break;
                                case SpawnObjectType.PREFAB:
                                    break;
                                case SpawnObjectType.CONTAINER:
                                    break;
                            }
                            spawnerSave.AddEntry(SpawnId, spawnableData.Id, liveEntity.Character.GetState());
                        });
                
                       
                    }
                   
                }
                else if (entitiesSelectionMode == EntitiesSelectionMode.RANDOM_FROM_AMOUNT)
                {
                    //TODO
                }
                SaveEntities();
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

        private void SaveEntities()
        {
            SpawnerSave spawnerSave = _savingSystem.GetModule<SpawnerSave>();
            foreach (var spawnedNpc in _spawnedNpcs)
            {
                spawnerSave.AddEntry(SpawnId, spawnedNpc.Key, spawnedNpc.Value.GetState());
            }
        }
    }
}