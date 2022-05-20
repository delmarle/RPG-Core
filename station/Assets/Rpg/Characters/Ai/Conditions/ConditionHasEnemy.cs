
namespace Station
{

    public class ConditionHasEnemy : FSMCondition
    {
        public bool Value;
        public override void OnEnter()
        {
            
        }

        public override bool EstimateCondition()
        {
            if (Value)
            {
                return _root.Owner.Memory.GetCurrentEnemy != null;
            }
            else
            {
                return _root.Owner.Memory.GetCurrentEnemy == null;
            }
        }

        public override void OnExit()
        {
            
        }
    }
}