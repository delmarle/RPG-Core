using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class UiSimpleCharacterCreationMaster : MonoBehaviour
    {
        #region FIELDS

        [SerializeField] private UiSelectFactionPanel _selectFactionPanel;
        [SerializeField] private UiSelectRacePanel _selectRacePanel;
        [SerializeField] private UiSelectClassPanel _selectClassPanel;
        [SerializeField] private UiValidateCharacterPanel _validateCharacterPanel;
        [SerializeField] private UiCharacterCreationListPanel _characterListPanel;
        
        private SimpleCharacterCreation _characterCreation;
        private RpgSettingsDb _gameSettingsDb;
        private FactionDb _factionDb;
        private RaceDb _raceDb;
        private PlayerClassDb _classDb;
        
        public UiPanel _currentPanel;
        //ui select faction
        #endregion

        #region Monobehaviour

        private void Start()
        {
            
            _gameSettingsDb = GameInstance.GetDb<RpgSettingsDb>();
            _factionDb = GameInstance.GetDb<FactionDb>();
            _raceDb = GameInstance.GetDb<RaceDb>();
            _classDb = GameInstance.GetDb<PlayerClassDb>();
            _characterCreation = (SimpleCharacterCreation)_gameSettingsDb.Get().CharacterCreation;
            if (_characterCreation == null)
            {
                Debug.LogError("the character creation module is incorrect.");
            }

            InitFactionPanel();
           
            SetCurrentPanel(_selectFactionPanel);
        }

        private void InitFactionPanel()
        {
            List<FactionModel> factionAvailable = new List<FactionModel>();
            foreach (var factionId in _characterCreation.Factions)
            {
                var factionModel = _factionDb.GetEntry(factionId);
                factionAvailable.Add(factionModel);
            }

            _selectFactionPanel.Setup(factionAvailable, OnSelectFaction);
        }
        private void InitRacePanel()
        {
            List<RaceModel> racesAvailable = new List<RaceModel>();
            foreach (var raceId in _characterCreation.Races)
            {
                var factionModel = _raceDb.GetEntry(raceId);
                racesAvailable.Add(factionModel);
            }
            
            _selectRacePanel.Setup(racesAvailable, OnSelectRace,_characterCreation.Genders, OnSelectGender, OnClickNextStep);
        }

        private void InitClassPanel()
        {
            string currentRaceId = _characterCreation.GetCurrentPlayerData.RaceId;
            _selectClassPanel.Setup(_characterCreation.Classes, currentRaceId, OnSelectClass, OnClickNextStep);
        }

        private void InitValidateCharPanel()
        {
            _validateCharacterPanel.Setup(_characterCreation, OnClickNextStep);
        }

        private void InitCharacterListPanel()
        {
            _characterListPanel.Setup(_characterCreation, OnStartEditCharacter, OnClickEditFaction, OnClickEnterGame);
        }

        private void OnClickEnterGame()
        {
            _characterCreation.EnterGame();
        }

        private void OnClickEditFaction()
        {
            SetCurrentPanel(_selectFactionPanel);
        }

        private void OnSelectFaction(FactionModel faction)
        {
            _characterCreation.SetCurrentFaction(faction);
        }
        
    
        private void OnSelectRace(int raceIndex)
        {
            string raceId = _raceDb.GetKey(raceIndex);
            _characterCreation.SetCurrentCharacterRace(raceId);
        }

        private void OnStartEditCharacter(int index)
        {
            _characterCreation.StartEditCharacter(index);
            InitRacePanel();
            SetCurrentPanel(_selectRacePanel);
        }
        
        private void OnSelectGender(int genderIndex)
        {
            string genderKey = _characterCreation.Genders[genderIndex].Key;
            _characterCreation.SetCurrentCharacterGender(genderKey);
        }
        
        private void OnSelectClass(int classSelectedId)
        {
            var classData = _characterCreation.Classes[classSelectedId];
            _characterCreation.SetCurrentCharacterClass(classData);
        }
        #endregion

        public void OnClickNextStep()
        {
            if (_currentPanel == (UiPanel)_selectFactionPanel)
            {
                InitCharacterListPanel();
                SetCurrentPanel(_characterListPanel);
                return;
            }
            
            if (_currentPanel == (UiPanel)_selectRacePanel)
            {
                InitClassPanel();
                SetCurrentPanel(_selectClassPanel);
                return;
            }
            
            if (_currentPanel == (UiPanel)_selectClassPanel)
            {
                InitValidateCharPanel();
                SetCurrentPanel(_validateCharacterPanel);
                return;
            }
            
            if (_currentPanel == (UiPanel)_validateCharacterPanel)
            {
                _characterCreation.OnCompleteCharacter();
                InitCharacterListPanel();
                SetCurrentPanel(_characterListPanel);
                return;
            }
        }

      
        #region Utils

        private void SetCurrentPanel(UiPanel nextPanel)
        {
            if (_currentPanel != null)
            {
                _currentPanel.Hide();
            }
            nextPanel.Show(0.3f);
            _currentPanel = nextPanel;
        }

        #endregion
        
    } 
}

