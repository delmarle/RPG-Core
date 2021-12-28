using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Station
{
    public class UiValidateCharacterPanel : UiPanel
    {

        [SerializeField] private UiRaceWidget _race;
        [SerializeField] private UiButton _class;
        
        [SerializeField] private TMP_InputField _nameInput;
        private SimpleCharacterCreation _creationAsset;
        private StationAction _completeCallBack;

        public void Setup(SimpleCharacterCreation asset, StationAction onComplete)
        {
            _nameInput.text = String.Empty;
            _creationAsset = asset;
            _completeCallBack = onComplete;
            //set views
            var character = asset.GetCurrentPlayerData;
            var raceDb = GameInstance.GetDb<RaceDb>();
            var raceMeta = raceDb.GetEntry(character.RaceId);
            _race.SetupRaceData(raceMeta);
            var classDb = GameInstance.GetDb<PlayerClassDb>();
            var classMeta = classDb.GetEntry(character.ClassId);
            _class.SetName(classMeta.Name.GetValue());
            _class.SetSecondText(classMeta.Description.GetValue());
            _class.SetIcon(classMeta.Icon);
        }
        
        public void OnClickNext()
        {
            if (_creationAsset.IsNameValid(_nameInput.text) == false)
            {
                return;
            }
            _creationAsset.SetCurrentCharacterName(_nameInput.text);
            _completeCallBack?.Invoke();
        }
    }
}

