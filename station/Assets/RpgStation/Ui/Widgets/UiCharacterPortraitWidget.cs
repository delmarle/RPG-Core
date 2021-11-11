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
    [SerializeField] private TextMeshProUGUI characterRace = null;
    [SerializeField] private TextMeshProUGUI characterClass = null;
    [SerializeField] private TextMeshProUGUI characterFaction = null;
    [SerializeField] private TextMeshProUGUI characterGender = null;
    [SerializeField] private UiVitalBarWidget[] _vitals = null;
    [SerializeField] private UiCharacterStatusWidget _statusWidget = null;
    [SerializeField] private BaseAnimation _animation = null;
    [SerializeField] private UnityEngine.UI.Button _button = null;
    [SerializeField] private Image _icon = null;

    private Dictionary<string, UiVitalBarWidget> _vitalSliders = new Dictionary<string, UiVitalBarWidget>();
    private BaseCharacter _character;
    private VitalsDb _vitalsDb;
    private StationAction<BaseCharacter> _buttonCallback;
    private const string STATE_ALIVE = "alive";
    private const string STATE_DEAD = "dead";

    #endregion
    
    #region subscription

    public void Setup(BaseCharacter character, StationAction<BaseCharacter> buttonCallback)
    {
      _vitalSliders.Clear();
      Unsubscribe();

      _buttonCallback = buttonCallback;
      if (_vitals?.Length != 0)
      {
        foreach (var slider in _vitals)
        {
          slider.gameObject.SetActive(false);
        }
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
        _vitalsDb = GameInstance.GetDb<VitalsDb>();
        _character = character;
        if (_statusWidget != null)
        {
          _statusWidget.Setup(_character);
        }

        
        SetMetaText(characterName, StationConst.CHARACTER_NAME);
        SetMetaText(characterRace, StationConst.RACE_ID);
        if (characterRace)
        {
          characterRace.text = _character.GetLocalizedRace();
        }
        if (characterClass)
        {
          characterClass.text=_character.GetLocalizedClass();
        }
        if (characterFaction)
        {
          characterFaction.text=_character.GetLocalizedFaction();
        }
        
        SetMetaText(characterGender, StationConst.GENDER_ID);
        SetMetaIcon(_icon, StationConst.ICON_DATA);


        Subscribe();


        int sliderIndex = 0;
        if (_vitals.Length != 0)
        {
          if (character.Stats != null && character.Stats.Vitals != null)
          {
            foreach (var vital in character.Stats.Vitals)
            {
              var instance = _vitals[sliderIndex];
              _vitalSliders.Add(vital.Key, instance);
              var dataEnergy = _vitalsDb.GetEntry(vital.Key);
              instance.Setup(dataEnergy);
              instance.gameObject.SetActive(true);
              sliderIndex++;
            }
          }
        }
        OnVitalsUpdated(character);
        SetStates();
      }
    }

    private void SetMetaText(TextMeshProUGUI field, string metaKey)
    {
      if (field)
      {
        field.text = (string) _character.GetMeta(metaKey);
      }
    }
    
    private void SetMetaIcon(Image field, string metaKey)
    {
      if (field)
      {
        field.sprite = (Sprite) _character.GetMeta(metaKey);
      }
    }
    
    private void OnDestroy()
    {
      Unsubscribe();
    }

    private void Subscribe()
    {
      _character.OnVitalsUpdated += OnVitalsUpdated;
      _character.OnDamaged += OnReceiveDamage;
      _character.OnHealed += OnHealed;
      _character.OnDie += OnDie;
      _character.OnRevived += OnRespawn;

      GameGlobalEvents.OnLeaderChanged.AddListener(OnLeaderChanged);

    }

    private void Unsubscribe()
    {
      if (_character == null) return;

      _buttonCallback = null;
      _character.OnVitalsUpdated -= OnVitalsUpdated;
      _character.OnDamaged -= OnReceiveDamage;
      _character.OnHealed -= OnHealed;
      _character.OnDie -= OnDie;
      _character.OnRevived -= OnRespawn;
      GameGlobalEvents.OnLeaderChanged.RemoveListener(OnLeaderChanged);
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
      SetStates();
    }

    private void OnDie(BaseCharacter character)
    {
      if (_animation && _character)
      {
        _animation.PlayState(STATE_DEAD);
      }
    }

    private void OnHealed(BaseCharacter character, VitalChangeData data)
    {
    }

    private void OnReceiveDamage(BaseCharacter character, VitalChangeData data)
    {
    }

    public void OnClick()
    {
      if (_character)
      {
        _buttonCallback?.Invoke(_character);
      }
    }

    public void SetSelected()
    {
      if (_button)
      {
        _button.interactable = false;
      }
    }

    public void SetNotSelected()
    {
      if (_button)
      {
        _button.interactable = true;
      }
    }


    private void SetStates()
    {
      if (_animation && _character)
      {
        _animation.PlayState(_character.IsDead? STATE_DEAD : STATE_ALIVE);
      }
    }
  }
}

