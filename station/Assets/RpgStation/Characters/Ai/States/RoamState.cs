using UnityEngine;
using UnityEngine.AI;

namespace Station
{
    public class RoamState : FSMState
    {
        #region FIELDS
        private const int  MAX_ATTEMPTS = 10;
        [SerializeField] private float _minDistance = 3;
        [SerializeField] private float _maxDistance = 12;
        [SerializeField] private bool _debugPath = false;
        [SerializeField] private float _extraDistance = 0.5f;
        //cached
        private Vector3 _originalPosition;
        private AiCharacterInput _input;
        private NavMeshPath _path;
        NavMeshHit hit;
        #endregion
        
        #region OVERRIDES
        protected override void OnInit()
        {
            _path = new NavMeshPath();
            _originalPosition = _root.Owner.GetFeet();
            _input = _root.Owner.GetComponent<AiCharacterInput>();
        }

        protected override void OnEnter()
        {
            _input.Stop();
        }

        protected override void OnExecute()
        {
            if (_path == null) return;
            if (_path.status != NavMeshPathStatus.PathComplete)
            {
            }

            if (_path.status == NavMeshPathStatus.PathPartial || !_input.Agent.pathPending && _input.Agent.remainingDistance <= _input.Agent.stoppingDistance + _extraDistance )
            {
                _input.SetDestination(PickUpValidDestination());
            }

            if (_debugPath)
            {
                for (int i = 0; i < _path.corners.Length - 1; i++)
                    Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.red);
            }

   
        }

        protected override void OnExit()
        {
            _input.Stop();
        }
        #endregion


        private Vector3 PickUpValidDestination()
        {
            Vector3 destination = _root.Owner.GetFeet();
            var min = _minDistance;
            var max = _maxDistance;
                 
            min = Mathf.Clamp(min, 0.01f, max);
            max = Mathf.Clamp(max, min, max);
            int attempts = 0;
            while ( attempts<= MAX_ATTEMPTS &&(( destination - _root.Owner.GetFeet() ).sqrMagnitude < min  || _path.status == NavMeshPathStatus.PathInvalid )) {
                destination = ( Random.insideUnitSphere * max ) + _root.Owner.GetFeet();
                var val = NavMesh.CalculatePath(_root.Owner.GetFeet(),destination,NavMesh.AllAreas, _path);
                attempts++;
            }
       
            if ( NavMesh.SamplePosition(destination, out hit, 900, NavMesh.AllAreas) )
            {
                return hit.position;
            }
            return _originalPosition;
        }
        
        public Vector3 RandomNavmeshLocation(float radius) {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
           
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
                finalPosition = hit.position;            
            }
            return finalPosition;
        }
    }

}

