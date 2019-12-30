using UnityEngine;

namespace Station
{
  /// <summary>
  /// runtime component to manage the stuff of the ability
  /// </summary>
  public class RuntimeAbility: CharacterAction
  {
    #region [ FIELDS ]

    private string _optionalId;
    private ActiveAbility _data;
    private int _rank;
    
    public int GetRankIndex()
    {
      return _rank;
    }

    #endregion

    public string OptionalId => _optionalId;

    public override CastingData CastingData => CurrentRank.Casting;

    public override InvokingData InvokingData => CurrentRank.Invoking;

    protected override float CooldownTime => CurrentRank.CoolDown;

    public ActiveAbility Data => _data;

    public AbilityRank CurrentRank => _data.Ranks[_rank];

    public void Initialize(ActiveAbility data,int rank, float coolDown,BaseCharacter user, string id)
    {
      _data = data;
      _rank = rank;
      _user = user;
      _optionalId = id;
      SetCoolDown(coolDown);
    }

    public override Sprite GetIcon()
    {
      return _data.Icon;
    }

    public override bool CanUse()
    {
      if (base.CanUse() == false)
        return false;

      var targeting = _data.Targeting;
      //check target requirement
      if (targeting.UsedAbilityTargeting != Targeting.AbilityTargeting.None)
      {
        //we need a target

        if (_user.Target == null)
        {
          //we have no target
          return false;
        }

        if (targeting.TargetRequiredState == Targeting.RequireTargetState.Dead)
        {
          if (!_user.Target.IsDead == false)
          {
            //the target should be dead, but its not
            return false;
          }
        }

        if (targeting.TargetRequiredState == Targeting.RequireTargetState.Alive)
        {
          if (_user.Target.IsDead == true)
          {
            //the target should be Alive, but its not
            return false;
          }
        }

        //check distance
        switch (_data.Targeting.UsedAbilityTargeting)
        {
          case Targeting.AbilityTargeting.Self:
            if (_user.Target != _user)
            {
              return false;
            }

            break;
          case Targeting.AbilityTargeting.SelfOrFriendly:
            if (_user.ResolveStance(_user) == Stance.Enemy)
            {
              //not an enemy
              return false;
            }

            break;
          case Targeting.AbilityTargeting.Friendly:
            if (_user.ResolveStance(_user) != Stance.Ally)
            {
              //not an friendly
              return false;
            }

            break;
          case Targeting.AbilityTargeting.Enemy:
            Debug.Log(_user.ResolveStance(_user));
            if (_user.ResolveStance(_user) != Stance.Enemy)
            {
              //not an enemy
              return false;
            }

            break;
          case Targeting.AbilityTargeting.NotSelf:
            if (_user.Target == _user)
            {
              return false;
            }

            break;
        }

        if (_data.Ranks[_rank].VitalsUsed.Count > 0)
        {
          foreach (var vitalToUse in _data.Ranks[_rank].VitalsUsed)
          {
            if (_user.Stats.Vitals.ContainsKey(vitalToUse.Id))
            {
              if (_user.Stats.Vitals[vitalToUse.Id].Current >= vitalToUse.Value)
              {
                return true;
              }
              else
              {
                return false; //not enough vital
              }
            }
            else
            {
              Debug.LogError("this character does not have the correct vital");
            }
          }
        }

      }
      return true;
    }


    protected override void OnInvokeEffect()
    {
      foreach (var vitalToUse in CurrentRank.VitalsUsed)
      {
        _user.Stats.Vitals[vitalToUse.Id].Decrease(vitalToUse.Value);
      }

     
      #region DRIVERS IMPLEMENTATION

      for (var index = 0; index < CurrentRank.ProjectileDrivers.Count; index++)
      {
        var driver = CurrentRank.ProjectileDrivers[index];
        //wait for delay
        Timer.Register(driver.Delay, () =>
        {
          if (driver.EffectPrefab)
          {
            //todo projectile object
            driver.Effects.ApplyEffects(_user, _user);
          }
          
        });
      }

      
    

      for (var index = 0; index < CurrentRank.DirectDrivers.Count; index++)
      {
        var driver = CurrentRank.DirectDrivers[index];
        //wait for delay
        Timer.Register(driver.Delay, () =>
        {
          if (driver.Target == DriverTarget.Self)
          {
            driver.Effects.ApplyEffects(_user, _user);
          }
          else if (driver.Target == DriverTarget.Target)
          {
            driver.Effects.ApplyEffects(_user, _user.Target);
          }
        });
      }

    

      #endregion
    }
  }
}
