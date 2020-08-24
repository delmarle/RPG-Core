

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weighted_Randomizer;
using Debug = UnityEngine.Debug;

namespace Station
{
    public class SpawningSystem : BaseSystem
    {
        #region FIELDS
        private GameSettingsDb _settingsDb;
 
        private SavingSystem _savingSystem;
        private SceneSystem _sceneSystem;
        
        private SceneSpawner[] _cacheSpawnsData;
        #endregion

        protected override void OnInit()
        {
           GameGlobalEvents.OnSceneLoadObjects.AddListener(OnEnterScene);
           _savingSystem = RpgStation.GetSystem<SavingSystem>();
           _sceneSystem = RpgStation.GetSystem<SceneSystem>();

        }

        protected override void OnDispose()
        {
            GameGlobalEvents.OnSceneLoadObjects.RemoveListener(OnEnterScene);
        }

        public void OnEnterScene()
        {
            _settingsDb = RpgStation.GetDb<GameSettingsDb>();

            var spawnerSave = _savingSystem.GetModule<SpawnerSave>();
            spawnerSave.Load(_sceneSystem.GetCurrentDestination().SceneName);
            _cacheSpawnsData = FindObjectsOfType<SceneSpawner>();
            if (_cacheSpawnsData == null) return;
            
            foreach (var spawner in _cacheSpawnsData)
            {
                spawner.Init(_settingsDb.Get().Mechanics);
                
            }

        }

        public List<IdIntegerValue> GetVitalsValues(NpcModel npc)
        {
            var vitalStatus = new List<IdIntegerValue>();
            if (npc.UseHealth)
            {
                var healthStatus = new IdIntegerValue(npc.HealthVital.Id,-1);
                vitalStatus.Add(healthStatus);
            }

            foreach (var energyData in npc.EnergyVitals)
            {
                var energyStatus = new IdIntegerValue(energyData.Id,-1);
                vitalStatus.Add(energyStatus);
            }

            return vitalStatus;
        }

        public SpawnData GetRandom(List<SpawnData> source)
        {
            IWeightedRandomizer<SpawnData> randomizer = new DynamicWeightedRandomizer<SpawnData>();
            foreach (var npcSpawnData in source)
            {
                //check if not blacklisted from save later
                randomizer.Add(npcSpawnData, npcSpawnData.Weight);
            }
       
            return randomizer.NextWithReplacement();
        }
        
        private void OnPlayerInstanced(GameObject instance)
        {

        }
    }

}

