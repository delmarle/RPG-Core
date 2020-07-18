using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class CombatState : FSMState
    {
        //get in range of the target
        //attack loop
        private RpgActionHandler _action;
        private AiCharacterInput _input;
        
        
        protected override void OnInit()
        {
          
            _input = _root.Owner.GetComponent<AiCharacterInput>();
        }

        protected override void OnEnter()
        {
            _action = (RpgActionHandler)_root.Owner.Action;
            _root.Owner.Target = _root.Owner.Memory.GetCurrentEnemy;
            _action?.WantAttack();
          
        }

        protected override void OnExecute()
        {
            _root.Owner.Target = _root.Owner.Memory.GetCurrentEnemy;
            _input.SetTarget(_root.Owner.Memory.GetCurrentEnemy.transform);
        }

        protected override void OnExit()
        {
            _action.SetAttacking(false);
        }
    }
}