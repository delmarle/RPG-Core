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
    

    #endregion

    protected virtual float CooldownTime
    {
      get { return 0; }
    }

    public virtual CastingData CastingData
    {
      get { return null; }
    }

    public virtual InvokingData InvokingData
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

    //extend for interact with door, open container, AutoLoop
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

      if (CastingData?.Option == ExitMode.CanceledByMovement)
      {
        ListenMovement();
      }
      _user.Action.DoAction(this);
      var casting = CastingData;
      if (casting != null)
      {
        if (_user.Action.OnStartCasting != null)
        {
          _user.Action.OnStartCasting.Invoke(this);
        }
        _castingTimer = Timer.Register(casting.Length, CompleteCasting);
      }
      else
      {
        InvokeEffect();
      }
    }

    public void CompleteCasting()
    {
      var casting = CastingData;

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

      var casting = CastingData;
      _user.Action.StopCasting();
      if (casting.Option == ExitMode.CanceledByMovement)
      {
        UnlistenMovement();
      }
      if(_castingTimer != null)_castingTimer.Cancel();
      if(_invokingTimer != null)_invokingTimer.Cancel();
      if (_user.Action.OnCancelCasting != null)
      {
        _user.Action.OnCancelCasting.Invoke(this);
      }
    }

    public void CancelInvoking()
    {
      _user.Action.DoFinishAction();
      var invoking = InvokingData;
      if(_invokingTimer != null)_invokingTimer.Cancel();
      if (invoking.Option == ExitMode.CanceledByMovement)
      {
        UnlistenMovement();
      }
    }


    private void InvokeEffect()
    {
      var invoking = InvokingData;
      if (invoking != null && invoking.Option == ExitMode.CanceledByMovement)
      {
        ListenMovement();
      }
      if (_user.Action.OnStartInvoking != null)
      {
        _user.Action.OnStartInvoking?.Invoke(this);
      }

      if (InvokingData == null)
      {
        _user.Action.OnCompleteInvoking?.Invoke(this);
      }
      else
      {
        _invokingTimer = Timer.Register(InvokingData.Length, () =>
        {
          if (_user.Action.OnCompleteInvoking != null)
          {
            if (invoking != null && invoking.Option == ExitMode.CanceledByMovement)
            {
              UnlistenMovement();
            }
            _user.Action.OnCompleteInvoking?.Invoke(this);
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
