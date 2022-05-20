using UnityEngine.AI;
using UnityEngine;

namespace Station
{
    [RequireComponent(typeof (NavMeshAgent))]
    public class AiCharacterInput : BaseInput
    {
        private MovementType _movementType = Station.MovementType.Auto;
        public NavMeshAgent Agent => _agent;
        [SerializeField] private NavMeshAgent _agent = null;
        [SerializeField] private Transform _target = null;
        private Vector3 _destination;
        private BaseCharacter _owner;
        
        protected override void OnActive(CoreCharacter character)
        {
            _owner = (BaseCharacter)character;
            _owner.OnDie-= OnDie;
            _owner.OnDie+= OnDie;
            NavMeshHit closestHit;

            if (NavMesh.SamplePosition(gameObject.transform.position, out closestHit, 500f, NavMesh.AllAreas))
            {
                gameObject.transform.position = closestHit.position;
            }
            else
            {
                Debug.LogError("Could not find position on NavMesh!");
            }

            _agent.enabled = true;
        }

 

        protected override void OnDeactivate(CoreCharacter character)
        {
            if (_owner)
            {
                _owner.OnDie-= OnDie;
            }
           
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
            _agent.ResetPath();
            _destination = transform.position;
            _target = null;
        }

        public override Vector3 Movement()
        {
            _agent.speed = _owner.Control.MaxHorizontalSpeed;
            _agent.updateRotation = false;
            if (_target != null)
            {
                if (_agent.isOnNavMesh == false)
                {
                    return Vector3.zero;
                }

                _destination = _target.position;
            }

            _agent.SetDestination(_destination);
            var distance = _agent.remainingDistance;
            if (distance > _agent.stoppingDistance)
            {
                return _agent.desiredVelocity;
            }

            return base.Movement();
        }

        public override int MovementType()
        {
            
            if (_movementType == Station.MovementType.Auto)
            {
            }

            var distance = _agent.isOnNavMesh?_agent.remainingDistance:0;
            if (distance < 10)
            {
                return 0;
            }

            if (distance < 25)
            {
                return 1;
            }
            return 2;
        }

        public void SetMovementType(MovementType moveType)
        {
            _movementType = moveType;
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
        
        private void OnDie(CoreCharacter character)
        {
            Stop();
        }
    }

    public enum MovementType
    {
        Auto, Walk, Run, Sprint
    }
}
