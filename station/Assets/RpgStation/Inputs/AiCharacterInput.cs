using UnityEngine.AI;
using UnityEngine;

namespace Station
{
    [RequireComponent(typeof (NavMeshAgent))]
    public class AiCharacterInput : BaseInput
    {
        [SerializeField] private NavMeshAgent _agent = null;
        [SerializeField] private Transform _target = null;

        protected override void OnActive(BaseCharacter character)
        {
            NavMeshHit closestHit;
 
            if (NavMesh.SamplePosition(gameObject.transform.position, out closestHit, 500f, NavMesh.AllAreas))
                gameObject.transform.position = closestHit.position;
            else
                Debug.LogError("Could not find position on NavMesh!");
            _agent.enabled = true;
        }

        protected override void OnDeactivate(BaseCharacter character)
        {
            _agent.enabled = false;
        }

        private void Start()
        {
            _agent.updateRotation = false;
            _agent.updatePosition = true;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public override Vector3 Movement()
        {
            if (_target != null)
            {
                if (_agent.isOnNavMesh == false)
                {
                    return Vector3.zero;
                }
      
                var distance = _agent.remainingDistance;
                _agent.SetDestination(_target.position);

                if (distance > _agent.stoppingDistance)
                    return _agent.desiredVelocity;
            }

            return base.Movement();
        }

        public override int MovementType()
        {
  
            var distance = _agent.isOnNavMesh?_agent.remainingDistance:0;
            if (distance < 10)
            {
                _agent.speed = 0.5f;
                return 0;
            }

            if (distance < 25)
            {
                _agent.speed = 1;
                return 1;
            }
            _agent.speed = 1.8f;
            return 2;
        }
    }
}
