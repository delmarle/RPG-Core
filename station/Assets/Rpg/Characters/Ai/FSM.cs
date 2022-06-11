using System;
using System.Collections.Generic;

using UnityEngine;

namespace Station
{
    public class FSM : CharacterBrain
    {
        [SerializeField] private FSMState _currentState;
        [SerializeField] private FSMState[] _states;
        public BaseCharacter Owner;
        
        public override void Setup(BaseCharacter owner)
        {
            Owner = owner;
            foreach (var state in _states)
            {
                state.Init(this);
            }
        }

        public override void TickBrain()
        {
            if (_currentState)
            {
                _currentState.Execute();
         
                foreach (var transition in _currentState.Transitions)
                { 
                    foreach (var condition in transition.Conditions)
                    {
                        if (condition.EstimateCondition())
                        {
                            ChangeState(transition.NextState);
                            return;
                        }
                    }
                        
                }
    
            }
        }
        

        void ChangeState(FSMState nextState)
        {
            if (_currentState)
            {
                _currentState.Exit();
            }

       
            _currentState = nextState;
        
            if (_currentState)
            {
                _currentState.Enter();
            }
        }
    }

    public abstract class FSMState: MonoBehaviour
    {
        protected FSM _root;
    
        public List<FSMStateTransition> Transitions = new List<FSMStateTransition>();

        public void Init(FSM root)
        {
            name = $"{GetType().Name}";
            _root = root;
            foreach (var transition in Transitions)
            {
                foreach (var condition in transition.Conditions)
                {
                    condition.Init(root);
                }
            }

            OnInit();
        }

        public void Execute()
        {
            OnExecute();
            EstimateTransitions();
        }

        private void EstimateTransitions()
        {
        }

        public void Enter()
        {
            name = $"{GetType().Name} [current state]";
            OnEnter();
            for (int i = 0; i < Transitions.Count; i++)
            {
                var transition = Transitions[i];
                for (int j = 0; j < transition.Conditions.Count; j++)
                {
                    transition.Conditions[j].OnEnter();
                }
            }
        }

        public void Exit()
        {
            name = $"{GetType().Name}";
            for (int i = 0; i < Transitions.Count; i++)
            {
                var transition = Transitions[i];
                for (int j = 0; j < transition.Conditions.Count; j++)
                {
                    transition.Conditions[j].OnExit();
                }
            }
            OnExit();
        }

        protected abstract void OnInit();
        
        protected abstract void OnEnter();

        protected abstract void OnExecute();

        protected abstract void OnExit();

    }
    
    [Serializable]
    public class FSMStateTransition
    {
        public List<FSMCondition> Conditions = new List<FSMCondition>();
        public FSMState NextState;
    }

    public abstract class FSMCondition:MonoBehaviour
    {
        protected FSM _root;

        public void Init(FSM root)
        {
            _root = root;
        }

        public abstract void OnEnter();

        public abstract bool EstimateCondition();

        public abstract void OnExit();
    }
}

