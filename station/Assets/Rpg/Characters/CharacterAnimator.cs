﻿using System.Collections;
using UnityEngine;

namespace Station
{
    public class CharacterAnimator : MonoBehaviour
{
  private static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
  private static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
  private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
  private static readonly int DeadState = Animator.StringToHash("Dead");
  private static readonly int IdleState = Animator.StringToHash("Idle");
  private static readonly int AbilityTrigger = Animator.StringToHash("AbilityTrigger");
  private static readonly int CastingState = Animator.StringToHash("CastingAbility");
  private static readonly int CastingId = Animator.StringToHash("CastingId");
  private static readonly int InvokingState = Animator.StringToHash("InvokingAbility");
  private static readonly int InvokingId = Animator.StringToHash("InvokingId");
  private static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
  private static readonly int AttackField = Animator.StringToHash("Combat");
  private bool _inCombat;
  private float _combat;
  private Animator _animator;
  private CharacterControl _characterControl;
  private BaseCharacter _baseCharacter;
  private SoundSystem _soundSystem;

  protected virtual void Awake()
  {
    _baseCharacter = GetComponent<BaseCharacter>();
    _animator = GetComponent<Animator>();
    if (_animator == null)
    {
      _animator = GetComponentInChildren<Animator>();
    }
    _characterControl = GetComponent<CharacterControl>();
    _soundSystem = GameInstance.GetSystem<SoundSystem>();

  }

  private IEnumerator Start()
  {
    while (_baseCharacter.Action == null)
    {
      yield return null;
    }
    Subscribe();
  }

  private void OnDestroy()
  {
    Unsubscribe();
  }

  private void Subscribe()
  {
    var abilities = _baseCharacter.Action;
    if (abilities != null)
    {
      abilities.OnStartCasting += OnStartCasting;
      abilities.OnCancelCasting += OnCancelCasting;
      abilities.OnCompleteCasting += OnCompleteCasting;
      abilities.OnStartAction += OnStartInvoking;
      abilities.OnFinishAction += OnCompleteInvoking;
      abilities.OnStartAction += OnAction;
      abilities.OnSwitchCombat += OnSwitchCombat;
    }

    _baseCharacter.OnDie+= OnDie;
    _baseCharacter.OnRevived += OnRevived;
  }

  private void Unsubscribe()
  {
    var abilities = _baseCharacter.Action;
    if (abilities != null)
    {
      abilities.OnStartCasting -= OnStartCasting;
      abilities.OnCancelCasting -= OnCancelCasting;
      abilities.OnCompleteCasting -= OnCompleteCasting;
      
      abilities.OnStartAction -= OnStartInvoking;
      abilities.OnFinishAction -= OnCompleteInvoking;
      abilities.OnSwitchCombat -= OnSwitchCombat;
      _baseCharacter.OnDie -= OnDie;
      _baseCharacter.OnRevived -= OnRevived;
    }
  }

  #region CASTING
  private void OnStartCasting(CharacterAction action)
  {
    bool hasValidAnimation = action.ActionFxData.AnimationId >= 0;
   
    if (action.ActionFxData.StartSound)
    {
      _soundSystem.PlaySound(action.ActionFxData.StartSound.name);
    }

    if (hasValidAnimation)
    {
      if(action.InvokingActionData != null)_animator.SetInteger(InvokingId, action.InvokingActionData.AnimationId);
      _animator.SetInteger(CastingId, action.ActionFxData.AnimationId);
      _animator.SetBool(CastingState, true);
      _animator.SetBool(InvokingState, false);
      _animator.SetTrigger(AbilityTrigger);
    }
  }
  
  private void OnCancelCasting(CharacterAction ability)
  {
    _animator.SetBool(CastingState, false);
    _animator.SetBool(InvokingState, false);
  }
  
  private void OnCompleteCasting(CharacterAction ability)
  {
    _animator.SetBool(CastingState, false); 
  }
  
  private void OnStartInvoking(CharacterAction ability)
  {
    _animator.SetBool(InvokingState, true);
  }
  
  private void OnCompleteInvoking(CharacterAction ability)
  {
    _animator.SetBool(InvokingState, false);
  }
  #endregion
  
  private void OnAction(CharacterAction characterAction)
  {
   
    if (characterAction is DefaultAttackAction)
    {
      //set weapon type
      _animator.SetTrigger(AttackTrigger);
    }
  }
  
  private void OnSwitchCombat(bool inCombat)
  {
    _inCombat = inCombat;
   
  }


  protected virtual void Update()
  {
    var changedCombatValue = _inCombat ? _combat  + Time.deltaTime : _combat - Time.deltaTime;
    _combat = Mathf.Clamp(changedCombatValue, 0, 1);
    _animator.SetFloat(AttackField, _combat);
    _animator.SetFloat(HorizontalSpeed, _characterControl.HorizontalSpeed);
    _animator.SetFloat(VerticalSpeed, _characterControl.VerticalSpeed);
    _animator.SetBool(IsGrounded, _characterControl.IsGrounded);
  }
  
  private void OnDie(CoreCharacter character)
  {
    _animator.Play(DeadState);
  }
  
  private void OnRevived(CoreCharacter character)
  {
    _animator.Play(IdleState);
  }
}
}
