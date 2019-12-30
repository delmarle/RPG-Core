﻿

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weighted_Randomizer;
using Debug = UnityEngine.Debug;

namespace Station
{
    public class NpcSystem : BaseSystem
    {
        #region FIELDS
        private GameSettingsDb _settingsDb;
        private DbSystem _dbSystem;
        private NpcDb _npcDb;
        private StationMechanics _mechanics;
        
        private Dictionary<string, List<BaseCharacter>> npcMap = new Dictionary<string, List<BaseCharacter>>();
        private NpcSpawns _cacheSpawns;
        #endregion
        //find reference all npc that can spawn
        //spawn and respawn
        //save
        protected override void OnInit()
        {
           GameGlobalEvents.OnSceneInitialize.AddListener(OnEnterScene);
           _dbSystem = RpgStation.GetSystemStatic<DbSystem>();

        }

        protected override void OnDispose()
        {
            GameGlobalEvents.OnSceneInitialize.RemoveListener(OnEnterScene);
        }

        public void OnEnterScene()
        {
            _settingsDb = _dbSystem.GetDb<GameSettingsDb>();
            _npcDb = _dbSystem.GetDb<NpcDb>();
            if (_mechanics == null)
            {
                _mechanics = _settingsDb.Get().Mechanics;
                _mechanics.Init(_station);
            }

            
            _cacheSpawns = FindObjectOfType<NpcSpawns>();
            if (_cacheSpawns == null) return;

            var spawnList = _cacheSpawns.Entries;
            for (int i = 0; i < _cacheSpawns.SpawnAmount; i++)
            {
                var randomNpc = GetRandom(spawnList);
                StartCoroutine("SpawnNpc", randomNpc.Id);
            }
        }

        public IEnumerator SpawnNpc(string npcId)
        {
            var data = _cacheSpawns.GetDataById(npcId);
            var npcModel = _npcDb.GetEntry(npcId);
            var characterData = new List<object>
            {
                npcModel.RaceId, npcId, "male"
            };
            string prefabId = npcModel.PrefabId;
            var op = _mechanics.OnCreateCharacter(new PlayerCharacterType(), characterData.ToArray(), OnPlayerInstanced, prefabId);

            if (op != null)
            {

                while (op.Value.IsDone == false)
                {
                    yield return null;
                }

                var instance = op.Value.Result;
                var component = instance.GetComponent<BaseCharacter>();
                
                if (component != null)
                {
                    _mechanics.OnBuildNpc(component, npcModel, npcId);

                    
                    component.transform.position = data.position;
                    component.Control.SetRotation(Quaternion.identity.eulerAngles);
                    component.AddMeta("identity", IdentityType.Npc.ToString());
                  
                    component.Stats.SetVitalsValue(GetVitalsValues(npcModel));
                    var t =RpgStation.GetSystemStatic<TeamSystem>().GetCurrentLeader();
                    component.GetInputHandler.SetAiInput(t?.transform);
                }
                else
                {
                    Debug.LogError("no component found for on player prefab");
                }
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

        public NpcSpawnData GetRandom(List<NpcSpawnData> source)
        {
            IWeightedRandomizer<NpcSpawnData> randomizer = new DynamicWeightedRandomizer<NpcSpawnData>();
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

