using System;
using System.Linq;
using Station.Data;
using UnityEngine;

namespace Station
{
    
    public class SceneSystem : BaseSystem
    {
        private SceneType _currentSceneType;
        private SavingSystem _savingSystem;
        private DbSystem _dbSystem;
        private ScenesDb _scenesDb;
        private GameSettingsDb _settingsDb;
        private PlayersSave _playersSave;
        private bool _isTraveling = false;
        public bool IsTraveling => _isTraveling;
        
        protected override void OnInit()
        {
            GameGlobalEvents.OnDataBaseLoaded.AddListener(OnDatabaseReady);
            _savingSystem = _station.GetSystem<SavingSystem>();
            _dbSystem = _station.GetSystem<DbSystem>();
          
            _playersSave = _savingSystem.GetModule<PlayersSave>();
        }
        
        protected override void OnDispose()
        {
            GameGlobalEvents.OnDataBaseLoaded.RemoveListener(OnDatabaseReady);
        }
        
        private void OnDatabaseReady()
        {
            _scenesDb = _dbSystem.GetDb<ScenesDb>();
            _settingsDb = _dbSystem.GetDb<GameSettingsDb>();
            //get character creation

            BaseCharacterCreation creation = _settingsDb.Get().CharacterCreation;
            creation.Init(_station);

            var module = _savingSystem.GetModule<PlayersSave>();
            int playerSaveCount = module.Value?.Count ?? 0;
        
            Debug.Log("The player save have "+playerSaveCount+" players");
            if (creation.HasData() == false)
            {
                creation.StartSequence();
            }
            else
            {
                //resume from save
               GameGlobalEvents.OnEnterGame.Invoke();
                TravelToZone(null);
            }
        }

        public void InjectDestinationInSave(DestinationModel destination)
        {
            
            if (_playersSave == null)
            {
                Debug.LogError("could not find player save");
            }
            else
            {
                _isTraveling = true;
                var sceneData = _scenesDb.GetEntry(destination.SceneId);
                var spawnsData = sceneData.SpawnPoints[destination.SpawnId];
                var spawns = spawnsData.Positions.ToList();

                if (_playersSave.Value.Count > spawns.Count)
                {
                    Debug.LogError("there is too many players and not enough points");
                    return;
                }

                foreach (var player in _playersSave.Value.Values)
                {
                    player.LastZoneId = destination.SceneId;
                    var spawn = spawns.RandomItem();
                    spawns.Remove(spawn);
                    player.LastPosition = spawn;
                    player.LastRotation = spawnsData.Direction;
                }
                
                _playersSave.Save();
            }
        }

        /// <summary>
        /// will start travel sequence where a new scene is loaded
        /// </summary>
        public void TravelToZone(TravelModel model)
        {
            
            if (model == null)
            {
                model = new TravelModel();
                var player = _playersSave.Value.First();
                model.SceneName = _scenesDb.GetEntry(player.Value.LastZoneId).VisualName;
            }

            if (model.SceneName == "")
            {
                Debug.LogError("destination was not set");
                return;
            }

            LoadingSceneTask task = new LoadingSceneTask(model, _settingsDb.Get().Mechanics.LoadingScreen);
            task.SetEndCallback(OnZoneLoadingDone);
            task.Execute();
        }

        private void OnZoneLoadingDone(ITemplateTask<bool> arg1, bool arg2, Exception arg3, object arg4)
        {
            _isTraveling = false;
        }
    }

    public class TravelModel
    {
        public string SceneName;
    }
}

