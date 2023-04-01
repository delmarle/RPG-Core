using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
  [Serializable]
  public class ActionHandler
  {
    #region [[ FIELDS ]]

    private const float COMBAT_EXIT_LENGTH = 10;
    public const int LINKED_AMOUNT = 4;
    protected List<RuntimeAbility> _abilities = new List<RuntimeAbility>();
    public DefaultAttackAction DefaultAttack = new DefaultAttackAction();
    public InteractionAction Interaction = new InteractionAction();
    public CharacterAction _currentAction;
    public Action OnAbilitiesChanged;
    protected BaseCharacter _character;
    public bool _inCombat;
    
    public Action<CharacterAction> OnStartCasting;
    public Action<CharacterAction> OnCompleteCasting;
    public Action<CharacterAction> OnCancelCasting;
    public Action<CharacterAction> OnStartAction;
    public Action<CharacterAction> OnFinishAction;
    public Action<CharacterAction, string> OnFailUseAction;

    public Action<bool> OnSwitchCombat;
    public Action OnMove;
  
  
    public Dictionary<string, List<BarSlotState>> ActionBinds;
    private float _combatTimeLeft = 0;
    #endregion

    public void Subscribe()
    {
      _character.OnDamaged+= OnDamaged;
    }

    public void Unsubscribe()
    {
      _character.OnDamaged-= OnDamaged;
    }

    private void OnDamaged(BaseCharacter character, VitalChangeData data)
    {
      if (_character.IsDead == false)
      {
        RefreshCombat();
      }
    }

    public void SetupDefaultAttack(AttackData data)
    {
      DefaultAttack.SetupData(data);
    }

    #region [[ ABILITIES ]]

    public List<RuntimeAbility> GetRuntimeAbilities()
    {
      return _abilities;
    }
    public virtual void SetAbilities(List<RuntimeAbility> list, BaseCharacter character)
    {
      DefaultAttack.Setup(character);
      Interaction.Setup(character);
      _character = character;
      _abilities.Clear();
      foreach (var entry in list)
      {
        _abilities.Add(entry);
      }

      OnAbilitiesChanged?.Invoke();
    }
    

    public void SetPassiveAbilities(List<RuntimePassiveAbility> list, BaseCharacter character)
    {
      
    }

    public  virtual void AddAbilities(List<RuntimeAbility> list)
    {
      foreach (var entry in list)
      {
        if (_abilities.Contains(entry) == false)
        {
          _abilities.Add(entry);
        }
      }
  
      if (OnAbilitiesChanged != null)
      {
        OnAbilitiesChanged.Invoke();
      }
    }
  
    public virtual void RemoveAbilities(List<RuntimeAbility> list)
    {
      foreach (var entry in list)
      {
        if (_abilities.Contains(entry))
        {
          _abilities.Remove(entry);
        }
      }
  
      if (OnAbilitiesChanged != null)
      {
        OnAbilitiesChanged.Invoke();
      }
    }

    #endregion
    #region [[ SET LINKED ]] 

    public void BuildBinds(Dictionary<string, List<BarSlotState>> binds)
    {
      ActionBinds = binds;
    }

    public LinkType GetLinkType(string category, int index)
    {
      if (ActionBinds.ContainsKey(category))
      {
        if (index < ActionBinds[category].Count)
        {
          var list = ActionBinds[category];
          var entry = list[index];
          return entry.Link;
        }
        
      }

      return LinkType.Empty;
    }

    public RuntimeAbility GetBindAbility(string category, int index)
    {
      if (ActionBinds.ContainsKey(category))
      {
        if (index < ActionBinds[category].Count)
        {
          var list = ActionBinds[category];
          var entry = list[index];
          if (entry.Link == LinkType.Ability)
          {
         

            var abilityIndex = ActionBinds[category][index].Index;
            if (_abilities.Count <= abilityIndex)
            {
              return null;
            }

            return _abilities[abilityIndex];
          }
        }
      }

      return null;
    }
      

    #endregion
    #region [[ ACTIONS ]]

    public void DoAction(CharacterAction action)
    {
      _currentAction = action;
      OnStartAction?.Invoke(action);
    }

    public void DoFinishAction()
    {
      OnFinishAction?.Invoke(_currentAction);
      _currentAction = null;
    }
    
    public void CancelCasting()
    {
      OnCancelCasting?.Invoke(_currentAction);
      _currentAction = null;
    }

    public bool IsUsingAction()
    {
      return _currentAction != null;
    }

    #endregion
    #region Combat
    protected void EnterCombat()
    {
      _combatTimeLeft = COMBAT_EXIT_LENGTH;
      _inCombat = true;
      OnSwitchCombat?.Invoke(true);
    }

    private void ExitCombat()
    {
      _inCombat = false;
      OnSwitchCombat?.Invoke(false);
    }

    public void RefreshCombat()
    {
      if (_inCombat == false)
      {
        EnterCombat();
      }
      else
      {
        _combatTimeLeft = COMBAT_EXIT_LENGTH;
      }
    }

    public void UpdateCombat()
    {
      if (_inCombat)
      {
        _combatTimeLeft -= Time.deltaTime;
        if (_combatTimeLeft <= 0)
        {
          ExitCombat();
        }
      }
    }

    #endregion
    #region [[ USE ABILITY ]]
  

    public virtual void TryUseAction(CharacterAction action)
    {
      if (_currentAction == action)
      {
        return;
      }

      if (action.CanUse())
      {
        if (_currentAction != null)
        {
          _currentAction.CancelCasting();
        }
  
        _currentAction = action;
        action.StartCasting();
      }
      else
      {
        if (OnFailUseAction != null)
        {
          OnFailUseAction(action, "Cant use");
        } 
      }
    }
  
    
    #endregion

    public void InvokeMove()
    {
      OnMove?.Invoke();
    }

    public virtual void UpdateLoop()
    {
    }
  }

}

