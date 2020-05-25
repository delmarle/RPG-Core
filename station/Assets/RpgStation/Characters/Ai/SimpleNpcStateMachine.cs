
using UnityEngine;
using Random = UnityEngine.Random;

namespace Station
{
    
    public class SimpleNpcStateMachine : MonoBehaviour
    {
        private BaseCharacter _baseCharacter;
        private AiCharacterInput _input;
        private bool reachedDestination = true;
        private Vector3 _destination;
        private float _roamingTime;
        private float _idleTime;
        
        public void Setup(BaseCharacter baseCharacter)
        {
            _baseCharacter = baseCharacter;
            _input = _baseCharacter.GetComponent<AiCharacterInput>();
            _input.SetStoppingDistance(1.5f);
            _baseCharacter.GetInputHandler.SetAiInput(null);
        }

        //idle
        //roam
        private void Update()
        {
            if (_idleTime > 0)
            {
                _idleTime -= Time.deltaTime;
                return;
            }

            if (reachedDestination)
            {
                _destination = transform.position + Random.insideUnitSphere * 10;
                _destination.y = transform.position.y;

                var navMeshDestination = _input.GetNearestPositionOnNavMesh(_destination, 10);
                
                _input.SetDestination(navMeshDestination);
                reachedDestination = false;
                _roamingTime = 0;
            }
            else
            {
                if (_input.Agent.isPathStale)
                {
                    CancelDestination();
                    return;
                }

                _roamingTime += Time.deltaTime;
                if (_roamingTime > 10)
                {
                    CancelDestination();
                    return;
                }

                var distance = Vector3.Distance(_baseCharacter.GetFeet(), _destination);

                if (distance < 3.1)
                {
                    CancelDestination();
                }
            }

           
        }
        private void CancelDestination()
        {
            reachedDestination = true;
            _input.Stop();
            _idleTime = 4;
        }
    }
}

