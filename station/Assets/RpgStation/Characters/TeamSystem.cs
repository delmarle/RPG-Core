using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    public class TeamSystem : BaseSystem
    {
        #region FIELDS

    

        private List<BaseCharacter> _characters = new List<BaseCharacter>();

        private BaseCharacter _leader;
        
        private SavingSystem _savingSystem;
        private GameSettingsDb _settingsDb;
        private CameraController _cameraController;
        private StationMechanics _mechanics;
        #endregion

        protected override void OnInit()
        {
            _savingSystem = GameInstance.GetSystem<SavingSystem>();
            GameGlobalEvents.OnSceneStartLoad.AddListener(OnStartLoadScene);
            GameGlobalEvents.OnSceneLoadObjects.AddListener(InitializeTeam);
        }

       
        protected override void OnDispose()
        {
            GameGlobalEvents.OnSceneStartLoad.RemoveListener(OnStartLoadScene);
            GameGlobalEvents.OnSceneLoadObjects.RemoveListener(InitializeTeam);
        }

        private void InitializeTeam()
        {
            StartCoroutine(InitializeTeamSequence());
        }
        
        private void OnStartLoadScene()
        {
            _leader = null;
            _characters = new List<BaseCharacter>();
        }


        private IEnumerator InitializeTeamSequence()
        {
            _settingsDb = GameInstance.GetDb<GameSettingsDb>();
            var playerClassDb = GameInstance.GetDb<PlayerClassDb>();
            var mechanics = _settingsDb.Get().Mechanics;
            var playersModule = _savingSystem.GetModule<PlayersSave>();

            foreach (var playerPair in playersModule.Value)
            {
                
                var player = playerPair.Value;
                var classData = playerClassDb.GetEntry(player.ClassId);
                var characterData = new List<object>
                {
                    classData, player, playerPair.Key
                };
                string prefabId = null;
                foreach (var entry in classData.AllowedRaces)
                {
                    if (entry.RaceId == player.RaceId)
                    {
                        prefabId = player.GenderId == "male"?entry.MaleAddressPrefab : entry.FemaleAddressPrefab;
                    }
                }

                BaseCharacterData baseData = new BaseCharacterData
                {
                    CharacterId =  playerPair.Key,
                    Gender = player.GenderId,
                    Identifier = player.ClassId,
                    RaceId = player.RaceId,
                    Position = player.LastPosition,
                    Rotation = player.LastRotation,
                    CharacterType = new PlayerCharacterType()
                };
                var task = mechanics.InstantiateCharacter(baseData, characterData.ToArray(), null, prefabId);
                while (task.GetResult() == null)
                {
                    yield return null;
                }
                AddTeamMember(task.GetResult());
            }

            yield return null;

            var first = _characters.FirstOrDefault();
         
            _mechanics = _settingsDb.Get().Mechanics;
            InitializeCamera();
            InitializeUi();
            yield return null;
            if (first != null)
            {
                SetLeader(first);
                first.AddMeta("identity", IdentityType.MainPlayer.ToString());
            }
           

            yield return null;
        }

        private void InitializeCamera()
        {
          
            _cameraController = _mechanics.CreateCamera();
        }

        private void InitializeUi()
        {
            _mechanics.CreateUi();
        }
        

        private void AddTeamMember(BaseCharacter character)
        {
            _characters.Add(character);
            GameGlobalEvents.OnCharacterAdded?.Invoke(character);
        }

        private void RemoveTeamMember(BaseCharacter character)
        {
            _characters.Add(character);
            GameGlobalEvents.OnCharacterRemoved?.Invoke(character);
        }

        private void SetLeader(BaseCharacter character)
        {
            if (_leader != null)
            {
                //set to ai

            }

            SetCharacterControllable(character);
            _cameraController.SetTarget(character.transform);
            _leader = character;
            GameGlobalEvents.OnLeaderChanged?.Invoke(character);
        }

        public void RequestLeaderChange(BaseCharacter character)
        {
            SetLeader(character);
        }

        private void SetCharacterControllable(BaseCharacter character)
        {
            _leader = character;
            if (_characters.Count == 0) return;
            foreach (var teamMember in _characters)
            {
                //we check if this is the member we want to control
                bool activateInput = teamMember == _leader;

                if (activateInput)
                {
                    teamMember.GetInputHandler.SetPlayerInput();
                }
                else
                {
                    //TODO change without target
                    teamMember.GetInputHandler.SetAiInput(character.transform);
                }
            }
        }

        public BaseCharacter GetCurrentLeader()
        {
            return _leader;
        }

        public List<BaseCharacter> GetTeamMembers()
        {
            return _characters;
        }
    }
}

