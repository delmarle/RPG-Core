using System;

using UnityEngine;

namespace Station
{
    public class CharacterAction
{
    #region [[ FIELDS ]] 

    private float _actionLength = 1;

    public Action OnStart;
    public Action OnComplete;
    private CoolDown _coolDownTimer = new CoolDown();
    private CoolDown _usingTimer = new CoolDown();
    protected BaseCharacter _user;
    protected ActionHandler _action;
    private Timer _castingTimer;
    private Timer _invokingTimer;
    private BaseVfxPlayer _castingEffectInstance;

    #endregion

    protected virtual float CooldownTime
    {
      get { return 0; }
    }

    public virtual ActionFxData ActionFxData
    {
      get { return null; }
    }

    public virtual ActionFxData InvokingActionData
    {
      get { return null; }
    }

    public virtual float CalculateActionLength()
    {
        return _actionLength;
    }

    public virtual Sprite GetIcon()
    {
      return null;
    }

    public virtual void Setup(BaseCharacter owner)
    {
        _user = owner;
        _action = _user.Action;
    }
    
    public virtual bool CanUse()
    {

        if (_user.Action.IsUsingAction())
        {
            return false;
        }
        return _coolDownTimer.IsReady();
    }

    public void Trigger()
    {
        SetCoolDown(CooldownTime);
        _user.Action.DoAction(this);
        InvokeEffect();
    }

 


    #region [[ COOLDOWN ]]
    public void SetCoolDown(float time)
    {
        _usingTimer.TriggerCoolDown(_actionLength, OnFinishUse);
        _coolDownTimer.TriggerCoolDown(time, OnCoolDownFinished);
        if (OnStart != null)
        {
            OnStart();
        }
    }

    private void OnFinishUse()
    {
        if (_user)
        {
            _user.Action.DoFinishAction();
        }
    }

    private void OnCoolDownFinished()
    {
        if (OnComplete != null)
        {
            OnComplete();
        }
    }

    public float CdTimeLeft()
    {
        return _coolDownTimer.TimeLeft;
    }
    #endregion

    
    
    #region ABILITY INVOKATIONS

    public void StartCasting()
    {
      if (_castingTimer != null && _castingTimer.IsDone == false)
      {
        CancelCasting();
      }

      if (ActionFxData?.Option == ExitMode.CanceledByMovement)
      {
        ListenMovement();
      }
      _user.Action.DoAction(this);
      var casting = ActionFxData;
      if (casting != null && casting.HasData)
      {
        if (_user.Action.OnStartCasting != null)
        {
          _user.Action.OnStartCasting.Invoke(this);
        }
        _castingTimer = Timer.Register(casting.Length, CompleteCasting);
        _castingEffectInstance?.Despawn();
        if (casting.Effect.EffectPrefab != null)
        {
          _castingEffectInstance = PoolSystem.Spawn<BaseVfxPlayer>(casting.Effect.EffectPrefab);
          _castingEffectInstance.PlayEffect(_user, _user.Target, null);
        }
      }
      else
      {
        InvokeEffect();
      }
    }

    public void CompleteCasting()
    {
      var casting = ActionFxData;
      _castingEffectInstance?.Despawn();
      if (casting.Option == ExitMode.CanceledByMovement)
      {
        UnlistenMovement();
      }
      if (_user.Action.OnCompleteCasting != null)
      {
        _user.Action.OnCompleteCasting.Invoke(this);
      }
  
      InvokeEffect();
    }

    public void CancelCasting()
    {
      _user.Action.DoFinishAction();
      _castingEffectInstance?.Despawn();
      var casting = ActionFxData;
      _user.Action.CancelCasting();
      if (casting.Option == ExitMode.CanceledByMovement)
      {
        UnlistenMovement();
      }
      if(_castingTimer != null)_castingTimer.Cancel();
      if(_invokingTimer != null)_invokingTimer.Cancel();
    
    }

    public void CancelInvoking()
    {
      _user.Action.DoFinishAction();
      var invoking = InvokingActionData;
      if(_invokingTimer != null)_invokingTimer.Cancel();
      if (invoking.Option == ExitMode.CanceledByMovement)
      {
        UnlistenMovement();
      }
    }


    private void InvokeEffect()
    {
      var invoking = InvokingActionData;
      if (invoking != null && invoking.Option == ExitMode.CanceledByMovement)
      {
        ListenMovement();
      }
      if (_user.Action.OnStartAction != null)
      {
        _user.Action.OnStartAction?.Invoke(this);
      }

      if (InvokingActionData == null)
      {
        _user.Action.OnFinishAction?.Invoke(this);
      }
      else
      {
        _invokingTimer = Timer.Register(InvokingActionData.Length, () =>
        {
          if (_user.Action.OnFinishAction != null)
          {
            if (invoking != null && invoking.Option == ExitMode.CanceledByMovement)
            {
              UnlistenMovement();
            }
            _user.Action.OnFinishAction?.Invoke(this);
          }
        });
      }

     
      SetCoolDown(CooldownTime);

      OnInvokeEffect();
    }

    protected virtual void OnInvokeEffect()
    {
    }

    #endregion
    

    private void ListenMovement()
    {
      _user.Action.OnMove += OnMoving;
    }
    
    private void UnlistenMovement()
    {
      _user.Action.OnMove -= OnMoving;
    }

    private void OnMoving()
    {
      CancelCasting();
      CancelInvoking();
    }
}
}
