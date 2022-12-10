﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Station
{
  public class Interactible : MonoBehaviour
  {
    #region [[FIELDS]]
    [SerializeField] private TextMeshProUGUI _interactionName = null;
    [SerializeField] private TextMeshProUGUI _description = null;
    [SerializeField] private Image _icon = null;
    public UnityEvent OnStartInteracting;
    public UnityEvent OnStopInteracting;
    [SerializeField] protected BaseAnimation _hint = null;
    [HideInInspector] public InteractionConfig Config;

    protected InteractionConfigsDb _interactionConfigsDb;
    protected UiPopup _cachedPopup;
    protected BaseCharacter _currentUser;
    #endregion

    private void Start()
    {
      Initialize();
      
      Setup();
    }

    private void OnDestroy()
    {
      RpgGameGlobalEvents.OnLeaderChanged.RemoveListener(OnLeaderChanged);
      Dispose();
    }

    private void Initialize()
    {

      RpgGameGlobalEvents.OnLeaderChanged.AddListener(OnLeaderChanged);
      _interactionConfigsDb = GameInstance.GetDb<InteractionConfigsDb>();
     

      Config = LoadInteractionConfig();
      if (Config == null) return;
      
      if (Config.ShowHintMode == ShowHintType.WhileInRange)
      {
        var trackable = gameObject.AddComponent<Trackable>();
        trackable.AddOnDetected(OnEnterRange);
        trackable.AddOnNotDetected(OnExitRange);
      }

      gameObject.layer = Config.Layer;
      HideVisual();
    }

    protected virtual InteractionConfig LoadInteractionConfig()
    {
      foreach (var conf in _interactionConfigsDb.Db)
      {
        if (GetType() == conf.Value.InteractibleType.Type)
        {
          return conf.Value;
        }
      }

      return null;
    }

    public virtual ActionFxData GetActionData()
    {
      return Config.actionFxData;
    }
    
    public virtual string GetInteractionName()
    {return String.Empty;}
    
    public virtual Sprite GetInteractionIcon()
    {return null;}
    private void OnLeaderChanged(BaseCharacter newPlayer)
    {
      if (_currentUser)
      {
        UnregisterCachedPopup();
        OnCancelInteraction(_currentUser);
      }
    }

    protected virtual void Setup()
    {
    }

    protected virtual void Dispose()
    {
    }

    public virtual void ShowVisual()
    {
      if (_hint != null)
      {
        BillboardSystem.AddBillBoard(_hint.transform, true);
        _hint.PlayState("Show");
      }
    }

    public virtual void HideVisual()
    {
      if (_hint != null)
      {
        BillboardSystem.RemoveBillBoard(_hint.transform);
        _hint.PlayState("Hide");
      }
    }
    
    
    public virtual bool CanUse(BaseCharacter character)
    {
     
      if (Config.TryInteractMode == InteractType.Tap ||
          Config.TryInteractMode == InteractType.HoverAndInput||
          Config.TryInteractMode == InteractType.UiInput)
      {
        var distanceFromCharacter = Vector3.Distance(transform.position, character.GetFeet());
        if (distanceFromCharacter > Config.InteractionRange)
        {
          if (Config.FailNotificationChannels.Any())
          {
            var dict = new Dictionary<string, object> {{UiConstants.TEXT_MESSAGE, $"Too far to interact with {GetObjectName()}"}};
            UiNotificationSystem.ShowNotification(Config.FailNotificationChannels, dict);
          }
          return false;
        }

      }

      if (_currentUser == character)
      {
        Debug.Log("you are already using this interaction");
        return false;
      }

      return true;
    }

    public virtual void Interact(BaseCharacter user)
    {
    
      OnStartInteracting.Invoke();
      _currentUser = user;
      switch (Config.CancelInteractionMode)
      {
        case CancelInteractionMode.None:
          _currentUser = null;
          break;
        case CancelInteractionMode.ByDistance:
          break;
        case CancelInteractionMode.ByMoving:
          user.Action.OnMove += OnCharacterMove; 
          break;
      }
    }

    private void OnCharacterMove()
    {
      UnregisterCachedPopup();
      OnCancelInteraction(_currentUser);
    }

    protected void CachePopup(UiPopup popup)
    {
      if (_cachedPopup)
      {
        if (_cachedPopup.IsVisible)
        {
          return;
        }
        Debug.LogError("already have a popup cached");
        
      }

      _cachedPopup = popup;
      RegisterCachedPopup();

    }

    private void RegisterCachedPopup()
    {
      if (_cachedPopup)
      {
        _cachedPopup.OnHide.AddListener(OnCachedPopupClose);
      }
    }
    private void UnregisterCachedPopup()
    {
      if (_cachedPopup)
      {
        _cachedPopup.OnHide.RemoveListener(OnCachedPopupClose);
        _cachedPopup = null;
      }
    }

    private void OnCachedPopupClose()
    {
      OnCancelInteraction(_currentUser);
    }

    #region CANCEL INTERACTION

    public virtual void OnCancelInteraction(BaseCharacter user)
    {
      if (_currentUser != null)
      {
        _currentUser.Action.OnMove -= OnCharacterMove; 
      }
      
      UnregisterCachedPopup();
      _currentUser = null;
      OnStopInteracting.Invoke();
    }

    #endregion

    #region [[ TriggersTypes ]]

    public void OnTap(BaseCharacter user)
    {
      
      if (Config.ShowHintMode == ShowHintType.Tap)
      {
        ShowVisual();
        if (Config.HideHintOptions.UseDistance)
        {
          InteractionHandler.AddDistanceCheck(this);
        }
      }

      if (Config.TryInteractMode == InteractType.Tap)
      {
        if (Config.HideHintOptions.UseDistance)
        {
        }

        user.Action.Interaction.TryInteract(this);
      }
    }

    #region [[ HOVERING ]]

    public bool CanBeHovered()
    {
      return Config.ShowHintMode == ShowHintType.Hover || Config.TryInteractMode == InteractType.HoverAndInput;
    }

    public void OnStartHover()
    {
      if (Config.ShowHintMode == ShowHintType.Hover)
      {
        ShowVisual();
      }
    }

    public void OnStopHover()
    {
      if (Config.ShowHintMode == ShowHintType.Hover)
      {
        HideVisual();
      }
    }

    #endregion
    
    #region [[ Distance tracking ]]

    private void OnEnterRange(Tracker tracker)
    {
      if (Config.ShowHintMode == ShowHintType.WhileInRange)
      {
        ShowVisual();
      }
    }

    private void OnExitRange(Tracker tracker)
    {
      if (Config.ShowHintMode == ShowHintType.WhileInRange)
      {
        HideVisual();
      }
    }
    #endregion

    #endregion
    
    #region UI

    protected void SetUiName(string interactionName)
    {
      if (_interactionName)
      {
        _interactionName.text = interactionName;
      }
    }
    
    protected void SetUiDescription(string interactionDesc)
    {
      if (_description)
      {
        _description.text = interactionDesc;
      }
    }
    
    protected void SetUiIcon(Sprite icon)
    {
      if (_icon)
      {
        _icon.sprite = icon;
      }
    }

    public virtual string GetObjectName()
    {
      return "object";
    }

    #endregion
  }
}

