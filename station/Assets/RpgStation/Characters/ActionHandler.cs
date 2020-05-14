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
    private Dictionary<int,BarSlotState> _linkedActivities = new Dictionary<int, BarSlotState>();
    protected List<RuntimeAbility> _abilities = new List<RuntimeAbility>();
    protected List<PassiveAbility> _passiveAbilities = new List<PassiveAbility>();
    public DefaultAttackAction DefaultAttack = new DefaultAttackAction();
    public InteractionAction Interaction = new InteractionAction();
    public Action OnAbilitiesChanged;
    protected BaseCharacter _character;
    public bool _inCombat;

    public Action<InteractionAction> OnStartInteracting;
    public Action<InteractionAction> OnCompleteInteracting;
    public Action<InteractionAction> OnCancelInteracting;
    public Action<CharacterAction> OnStartCasting;
    public Action<CharacterAction> OnCompleteCasting;
    public Action<CharacterAction> OnCancelCasting;
    public Action<CharacterAction> OnStartInvoking;
    public Action<CharacterAction> OnCompleteInvoking;
    public Action<CharacterAction> OnStartAction;
    public Action OnFinishAction;
    public Action<bool> OnSwitchCombat;
    public Action OnMove;
  
    public RuntimeAbility _castingAbility;
    public CharacterAction _currentAction;
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
      _currentAction = null;
      _castingAbility = null;
      OnFinishAction?.Invoke();
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
  
    public void StopCasting()
    {
      _castingAbility = null;
      
    }
    
    public virtual void TryUseAbility(RuntimeAbility ability)
    {
      if (_castingAbility == ability)
      {
        return;
      }

      if (ability.CanUse())
      {
        if (_castingAbility != null)
        {
          _castingAbility.CancelCasting();
        }
  
        _castingAbility = ability;
        ability.StartCasting();
      }
      else
      {
        Debug.Log("failed");
        //FAILED
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

