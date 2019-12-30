using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Station
{
    [Serializable]
    public class AttackData
    {
        public DamageEffect BaseDamage = new DamageEffect();
        public float CriticalChance;
        public float Range = 2;
        public float Speed = 1.4f;
    }

    public class DefaultAttackAction : CharacterAction
    {
        private AttackData Data;

        public void SetupData(AttackData data)
        {
            Data = data;
        }

        public override float CalculateActionLength()
        {
            var calculation = _user.Calculator;
            return Data.Speed * calculation.GetActionSpeedMultiplier("attack");
        }

        public override bool CanUse()
        {
            var target = _user.Target;
            
            if (target && target.IsDead)
            {
                return false;
            }

            return base.CanUse();
        }

        protected override void OnInvokeEffect()
        {
            var target = _user.Target;
            if (target == null)
            {
                return;
            }

            if (_user.ResolveStance(target) == Stance.Ally)
            {
                return;
            }

            var distance = Vector3.Distance(_user.transform.position, target.transform.position);
            if (distance > Data.Range)
            {
                //out of range
                return;
            }
            Data.BaseDamage.ApplyEffect(_user, target);
        }
    }
}


