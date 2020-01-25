using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    
    public class UiCharacterPortraitWidget : MonoBehaviour
    {
#region FIELDS

        [SerializeField] private TextMeshProUGUI characterName = null;
        [SerializeField] private TextMeshProUGUI characterClass = null;
        [SerializeField] private UiVitalBarWidget[] _vitals = null;
        [SerializeField] private UiCharacterStatusWidget _statusWidget = null;

        private BaseCharacter _cacheCharacter = null;
#endregion
        
      #region [[ FIELDS ]]

    [SerializeField] private UnityEngine.UI.Button _button = null;
    [SerializeField] private Image _icon = null;

    private Dictionary<string, UiVitalBarWidget> _vitalSliders = new Dictionary<string, UiVitalBarWidget>();
    private BaseCharacter _character;
    private VitalsDb _vitalsDb;
    private TeamSystem _teamSystem;
    #endregion

    #region subscription

    public void Setup(BaseCharacter character)
    {
      _vitalSliders.Clear();
      Unsubscribe();
      foreach (var slider in _vitals)
      {
        slider.gameObject.SetActive(false);
      }
      
      if (character == null)
      {
        if (_statusWidget != null)
        {
          _statusWidget.Setup(null);
        }
        
        characterName.text = string.Empty;
        characterClass.text = string.Empty;
       
        
        _character = null;
      }
      else
      {
        var dbSystem = RpgStation.GetSystemStatic<DbSystem>();
        _vitalsDb = dbSystem.GetDb<VitalsDb>();
        _teamSystem = RpgStation.GetSystemStatic<TeamSystem>();
        _character = character;
        if (_statusWidget != null)
        {
          _statusWidget.Setup(_character);
        }

        characterName.text = _character.GetName();
        characterClass.text = _character.GetMeta("class");
        Subscribe();
       
      
        int sliderIndex = 0;
        if (character.Stats != null && character.Stats.Vitals != null)
        {
          foreach (var vital in character.Stats.Vitals)
          {
            var instance = _vitals[sliderIndex];
            _vitalSliders.Add(vital.Key,instance);
            var dataEnergy = _vitalsDb.GetEntry(vital.Key);
            instance.Setup(dataEnergy);
            instance.gameObject.SetActive(true);
            sliderIndex++;
          }
        }
        //_icon.sprite = character.ClassStaticData.Icon;
        OnVitalsUpdated(character);
      }

      
    }

    private void OnDestroy()
    {
      Unsubscribe();
    }

    private void Subscribe()
    {
      _character.OnCharacterInitialized += OnCharacterInitialized;
      _character.OnVitalsUpdated += OnVitalsUpdated;
      _character.OnDamaged += OnReceiveDamage;
      _character.OnHealed += OnHealed;
      _character.OnDie += OnDie;
      _character.OnRevived += OnRespawn;

      TeamSystem.OnLeaderChanged.AddListener(OnLeaderChanged);

    }

    public void Unsubscribe()
    {
      if (_character == null) return;
        
      _character.OnCharacterInitialized -= OnCharacterInitialized;
      _character.OnVitalsUpdated -= OnVitalsUpdated;
      _character.OnDamaged -= OnReceiveDamage;
      _character.OnHealed -= OnHealed;
      _character.OnDie -= OnDie;
      _character.OnRevived -= OnRespawn;
      TeamSystem.OnLeaderChanged.RemoveListener(OnLeaderChanged);
    }


    private void OnLeaderChanged(BaseCharacter character)
    {
      if (_button == null) return;
      
      if (_character == character)
      {
        SetSelected();
      }
      else
      {
        SetNotSelected();
      }
    }

    #endregion

    private void OnVitalsUpdated(BaseCharacter character)
    {
      var playerStats = character.Stats;
      if (playerStats == null) return;
      foreach (var vital in playerStats.Vitals)
      {
        if (_vitalSliders.ContainsKey(vital.Key))
        {
          _vitalSliders[vital.Key].SetVitalValue(vital.Value.Current, vital.Value.MaximumValue);
        }
      }
    }
    
    private void OnRespawn(BaseCharacter character)
    {
    }

    private void OnDie(BaseCharacter character)
    {
    }

    private void OnHealed(BaseCharacter character, VitalChangeData data)
    {
    }

    private void OnReceiveDamage(BaseCharacter character, VitalChangeData data)
    {
    }

    private void OnCharacterInitialized(BaseCharacter character)
    {
    //  var playerClass = Resource.PlayerClassesDatabase.GetEntry(character.CharacterData.GetClass());
    //  _icon.sprite = playerClass.Icon;
    }

    public void OnClick()
    {
      if (_character)
      {
        _teamSystem.RequestLeaderChange(_character);
      }
    }

    public void SetSelected()
    {
      _button.interactable = false;
    }

    public void SetNotSelected()
    {
      _button.interactable = true;
    }
    }
}

