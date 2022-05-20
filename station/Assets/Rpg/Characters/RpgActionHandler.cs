using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [Serializable]
    public class RpgActionHandler : ActionHandler
    {
        #region FIELDS
        private const float GLOBAL_COOL_DOWN = 0.1f;
        public bool _attacking = false;
        public CharacterAction _nextAction;
        public float _timeBeforeNextAction;
        #endregion

        public List<RankedTimeIdSave> GetAbilitiesState()
        {
            List<RankedTimeIdSave> stateList = new List<RankedTimeIdSave>();

            for (var index = 0; index < _abilities.Count; index++)
            {
                var runtimeAbility = _abilities[index];
                RankedTimeIdSave ab = new RankedTimeIdSave(runtimeAbility.OptionalId, runtimeAbility.GetRankIndex(),
                    runtimeAbility.CdTimeLeft());
                stateList.Add(ab);
            }

            return stateList;
        }

        #region [[ Actions & combat bridges ]]

        public override void UpdateLoop()
        {
            UpdateActions();
        }

        public void SetAttacking(bool val)
        {
            _attacking = val;
        }

        private void UpdateActions()
        {
            if (_timeBeforeNextAction >= 0)
            {
                _timeBeforeNextAction -= Time.deltaTime;
            }


            if (_attacking)
            {
                if (_character.Control.IsMoving())
                {
                    return;
                }

                var target = _character.Target;
                //if in range && not moving

                if (target)
                {
                    _character.Control.LerpFaceTarget(target.transform.position);
                }
            }

            if (_timeBeforeNextAction <= 0)
            {
                if (_currentAction != null)
                {
                    return;
                }

                //do an action from queue
                if (_nextAction != null && _nextAction.CanUse())
                {
                    //action left to do
                    base.TryUseAction((RuntimeAbility) _nextAction);
                    _nextAction = null;
                }
                else
                {
                    if (_attacking)
                    {
                        //no action to do but we are ready
                        if (DefaultAttack.CanUse())
                        {
                            RefreshCombat();
                            var actionLength = DefaultAttack.CalculateActionLength();
                            _timeBeforeNextAction = actionLength + GLOBAL_COOL_DOWN;
                            DefaultAttack.SetCoolDown(0);
                            DefaultAttack.Trigger();
                        }
                    }
                }
            }
        }

        #endregion

        public void WantAttack()
        {
            SetAttacking(true);
            if (_inCombat == false)
            {
                EnterCombat();
            }

            if (_character.Target == null)
            {
                var enemies = ScanForEnemy(_character.transform.position);
                _character.Target = enemies.FirstOrDefault();
            }

            if (_character.Target != null)
            {
                //initiate attacking
            }
        }

        private List<BaseCharacter> ScanForEnemy(Vector3 sourcePosition)
        {
            var characterPosition = _character.transform.position;
            RaycastUtils.SphereCastSearchComponentsAroundSource(_character, characterPosition, 4, out var characters);

            var filtered = characters.Where(x => _character.ResolveStance(x) != Stance.Ally).ToList();
            filtered.Sort(delegate(BaseCharacter a, BaseCharacter b)
            {
                float squaredRangeA = (a.transform.position - sourcePosition).sqrMagnitude;
                float squaredRangeB = (b.transform.position - sourcePosition).sqrMagnitude;
                return squaredRangeA.CompareTo(squaredRangeB);
            });
            return filtered;
        }
    }
}