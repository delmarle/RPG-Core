using System;
using System.Linq;
using UnityEngine;

namespace Station
{
    public class RpgSceneSystem: BaseSystem
    {
        private TravelModel _currentDestination;
        public TravelModel GetCurrentDestination() => _currentDestination;

        private RpgSettingsDb _rpgSettingsDb;

        private bool _isTraveling = false;
        public bool IsTraveling => _isTraveling;
        private PlayersSave _playersSave;
        private SavingSystem _savingSystem;
        private ScenesDb _scenesDb;
        private GameSettingsDb _settingsDb;
        protected override void OnInit()
        {
           
        }
        
        protected override void OnDispose()
        {
        }

        protected override void OnDataBaseReady()
        {
            _savingSystem = GameInstance.GetSystem<SavingSystem>();
            _scenesDb = GameInstance.GetDb<ScenesDb>();
            _settingsDb = GameInstance.GetDb<GameSettingsDb>();
            _playersSave = _savingSystem.GetModule<PlayersSave>();
            _scenesDb = GameInstance.GetDb<ScenesDb>();
            _settingsDb = GameInstance.GetDb<GameSettingsDb>();
            _rpgSettingsDb = GameInstance.GetDb<RpgSettingsDb>();
            //get character creation

            BaseCharacterCreation creation = _rpgSettingsDb.Get().CharacterCreation;
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
            _isTraveling = true;
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

            _currentDestination = model;
            LoadAreaTask task = new LoadAreaTask(model, _settingsDb.Get().LoadingScreen);
            task.SetEndCallback(OnZoneLoadingDone);
            task.Execute();
        }

        private void OnZoneLoadingDone(ITemplateTask<bool> arg1, bool arg2, Exception arg3, object arg4)
        {
            _isTraveling = false;
        }
        
        private void OnZoneSceneDone(ITemplateTask<bool> arg1, bool arg2, Exception arg3, object arg4)
        {
            _isTraveling = false;
        }

    }
}