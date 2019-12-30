using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    public class TeamSystem : BaseSystem
    {
        #region FIELDS

        public static StationEvent<BaseCharacter> OnCharacterAdded = new StationEvent<BaseCharacter>();
        public static StationEvent<BaseCharacter> OnCharacterRemoved = new StationEvent<BaseCharacter>();
        public static StationEvent<BaseCharacter> OnLeaderChanged = new StationEvent<BaseCharacter>();

        private List<BaseCharacter> _characters = new List<BaseCharacter>();

        private BaseCharacter _leader;
        
        private SavingSystem _savingSystem;
        private DbSystem _dbSystem;
        private GameSettingsDb _settingsDb;
        private CameraController _cameraController;
        private StationMechanics _mechanics;
        #endregion

        protected override void OnInit()
        {
            _savingSystem = _station.GetSystem<SavingSystem>();
            _dbSystem = _station.GetSystem<DbSystem>();
            GameGlobalEvents.OnSceneStartLoad.AddListener(OnStartLoadScene);
            GameGlobalEvents.OnSceneReady.AddListener(InitializeTeam);
        }

       
        protected override void OnDispose()
        {
            Debug.Log("team dispose");
            GameGlobalEvents.OnSceneStartLoad.RemoveListener(OnStartLoadScene);
            GameGlobalEvents.OnSceneReady.RemoveListener(InitializeTeam);
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


        IEnumerator InitializeTeamSequence()
        {
            _settingsDb = _dbSystem.GetDb<GameSettingsDb>();
            var playerClassDb = _dbSystem.GetDb<PlayerClassDb>();
            var mechanics = _settingsDb.Get().Mechanics;
            var playersModule = _savingSystem.GetModule<PlayersSave>();
            mechanics.Init(_station);

            foreach (var playerPair in playersModule.Value)
            {
                
                var player = playerPair.Value;
                var classData = playerClassDb.GetEntry(player.ClassId);
                var characterData = new List<object>
                {
                    player.RaceId, player.ClassId, player.GenderId
                };
                string prefabId = null;
                foreach (var entry in classData.AllowedRaces)
                {
                    if (entry.RaceId == player.RaceId)
                    {
                        prefabId = player.GenderId == "male"?entry.MaleAddressPrefab : entry.FemaleAddressPrefab;
                    }
                }

                var op = mechanics.OnCreateCharacter(new PlayerCharacterType(), characterData.ToArray(), OnPlayerInstanced, prefabId);

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
                        mechanics.OnBuildPlayer(component, player, classData);
                 

                        component.transform.position = player.LastPosition;
                        component.Control.SetRotation(player.LastRotation);
                        component.transform.rotation = Quaternion.Euler(player.LastRotation);
                        component.AddMeta(PlayersSave.PLAYER_KEY, playerPair.Key);
                        component.AddMeta("identity", IdentityType.TeamMember.ToString());
                        component.Stats.SetVitalsValue(player.VitalStatus);
                        AddTeamMember(component);
                    }
                    else
                    {
                        Debug.LogError("no component found for on player prefab");
                    }
                }
            }

            yield return null;

            var first = _characters.First();
         
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

        private void OnPlayerInstanced(GameObject instance)
        {

        }

        private void AddTeamMember(BaseCharacter character)
        {
            _characters.Add(character);
            OnCharacterAdded?.Invoke(character);
        }

        private void RemoveTeamMember(BaseCharacter character)
        {
            _characters.Add(character);
            OnCharacterRemoved?.Invoke(character);
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
            OnLeaderChanged?.Invoke(character);

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

