using UnityEngine.AI;
using UnityEngine;

namespace Station
{
    [RequireComponent(typeof (NavMeshAgent))]
    public class AiCharacterInput : BaseInput
    {
        public NavMeshAgent Agent => _agent;
        [SerializeField] private NavMeshAgent _agent = null;
        [SerializeField] private Transform _target = null;
        private Vector3 _destination;
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

        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
        }

        public void Stop()
        {
            _destination = transform.position;
        }

        public override Vector3 Movement()
        {
            _agent.updateRotation = false;
            if (_target != null)
            {
                if (_agent.isOnNavMesh == false)
                {
                    return Vector3.zero;
                }

                _destination = _target.position;
                var distance = _agent.remainingDistance;
                _agent.SetDestination(_target.position);

                if (distance > _agent.stoppingDistance)
                {
                    return _agent.desiredVelocity;
                }
            }
            else
            {
                _agent.SetDestination(_destination);
                var distance = _agent.remainingDistance;
                if (distance > _agent.stoppingDistance)
                {
                    return _agent.desiredVelocity;
                }
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

        public void SetStoppingDistance(float distance)
        {
            _agent.stoppingDistance = distance;
        }

        public Vector3 GetNearestPositionOnNavMesh(Vector3 direction, float maxDistance)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(direction, out hit, Random.Range(0f, maxDistance), 1);
          
            return hit.position;
        }
        
       
    }
}
