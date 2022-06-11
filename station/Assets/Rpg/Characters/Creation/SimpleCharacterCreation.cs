using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
    public class SimpleCharacterCreation : BaseCharacterCreation
    {
        #region FIELDS

        [SerializeField] private string _destinationId;
        [SerializeField] private List<ItemStack> _defaultItems = new List<ItemStack>();

        public string scene;
        public List<string> Classes;
        public List<string> Factions;
        public List<string> Races;
        public List<GenderModel> Genders;
        
        private List<PlayersData> _players = new List<PlayersData>();
        private int _currentEditedCharacter;
        public List<PlayersData> GetPlayers => _players;
        private PlayersData _editingCharacter => _players[_currentEditedCharacter];
        public PlayersData GetCurrentPlayerData => _editingCharacter;
        private FactionModel _faction;

        private RpgGameSettingsModel _gameSettings = null;
        private SceneSystem _sceneSystem;
        private SavingSystem _savingSystem;
        private FactionDb _factionDb;
        private PlayerClassDb _playerClassDb;
        private ScenesDb _scenesDb;
        #endregion

        private void Awake()
        {
            ClearData();
        }

        public override void Init(GameInstance station)
        {
            ClearData();
            _playerClassDb = GameInstance.GetDb<PlayerClassDb>();
            _sceneSystem = GameInstance.GetSystem<SceneSystem>();
            _savingSystem = GameInstance.GetSystem<SavingSystem>();
            var gameSettingsDb = GameInstance.GetDb<RpgSettingsDb>();
            _factionDb =  GameInstance.GetDb<FactionDb>();
            _scenesDb =  GameInstance.GetDb<ScenesDb>();
            
            _gameSettings = gameSettingsDb.Get();
            for (int i = 0; i < MaxSlots(); i++)
            {
                _players.Add(new PlayersData());
            }
        }

        private void ClearData()
        {
            _currentEditedCharacter = 0;
            _faction = null;
            _players.Clear();
        }

        public override bool HasData()
        {
            var module = _savingSystem.GetModule<PlayersSave>();
            int playerSaveCount = module.Value?.Count ?? 0;
            return playerSaveCount > 0;
        }

        public override void StartSequence()
        {
            GameGlobalEvents.OnEnterGame.Invoke();
            _sceneSystem.LoadNormalScene(scene, null);
            while (_sceneSystem.IsLoadingScene)
            {
                 return;
            }
        }

        public override string Description()
        {
            return "Simplified version of character creation \n" +
                   "-Select your faction\n" +
                   "-Select your race & gender\n" +
                   "-Select your class";
        }

        public void StartEditCharacter(int index)
        {
            _currentEditedCharacter = index;
        }
        public void SetCurrentFaction(FactionModel faction)
        {
            _faction = faction;
            var factionId = _factionDb.GetKey(_faction);
            foreach (var player in _players)
            {
                player.FactionId = factionId;
            }
        }

        #region edit character
      
        
        public void SetCurrentCharacterName(string characterName)
        {
            _editingCharacter.Name = characterName;
        }
        
        public void SetCurrentCharacterRace(string race)
        {
            _editingCharacter.RaceId = race;
        }
        
        public void SetCurrentCharacterGender(string gender)
        {
            _editingCharacter.GenderId = gender;
        }

        
        public void SetCurrentCharacterClass(string characterClass)
        {
            _editingCharacter.ClassId = characterClass;
        }
        
        public void OnCompleteCharacter()
        {
            var factionId = _factionDb.GetKey(_faction);
            _editingCharacter.FactionId = factionId;
            var classMeta = _playerClassDb.GetEntry(_editingCharacter.ClassId);
            _players[_currentEditedCharacter] = CharacterUtils.CreateCharacterSave
            (
                classMeta,
                _editingCharacter.Name,
                _editingCharacter.RaceId,
                _editingCharacter.ClassId,
                _editingCharacter.GenderId,
                _editingCharacter.FactionId,
                _destinationId,
                _editingCharacter.LastPosition
                );
        }

        public void EnterGame()
        {
            var module = _savingSystem.GetModule<PlayersSave>();
            module.Value = new Dictionary<string, PlayersData>();
            foreach (var player in _players)
            {
                module.AddPlayer(Guid.NewGuid().ToString(), player);
            }
        
            var destinationModel = new DestinationModel
            {
                SceneId = _destinationId, 
                SpawnId = 0
            };
            var rpgSceneSystem =  GameInstance.GetSystem<RpgSceneSystem>();
            rpgSceneSystem.InjectDestinationInSave(destinationModel);
            module.Save();
            //go to zone
            var sceneEntry = _scenesDb.GetEntry(_destinationId);
            TravelModel model = new TravelModel {SceneName = sceneEntry.VisualName};

            InventoryUtils.CreatePlayerInventory(_defaultItems);
            
            GameGlobalEvents.OnEnterGame.Invoke();
            rpgSceneSystem.TravelToZone(model);
        }
        
      
        public bool AllCharacterCreated()
        {
            if (_gameSettings.CharacterCreatedCount == _players.Count)
            {
                foreach (var player in _players)
                {
                    if (string.IsNullOrEmpty(player.Name)) return false;
                }
            }
            else
            {
                return false;
            }
            
            return true;
        }

        public int MaxSlots()
        {
            return _gameSettings.CharacterCreatedCount;
        }
        
        #endregion

        public bool IsNameValid(string nameChecked)
        {
            if (string.IsNullOrEmpty(nameChecked)) return true;

            foreach (var player in _players)
            {
                if (player.Name == nameChecked) return true;
            }
            return true;
        }
    }
}