using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    
    public class AiSimulationSystem : BaseSystem
    {
        #region FIELDS

        private const float EXECUTION_FREQUENCY = 0.25f;
        private List<CharacterMemoryHandler> _simulatedCharacters = new List<CharacterMemoryHandler>();
        private float _timeLeft;
        private static AiSimulationSystem _instance;
        #endregion
        protected override void OnInit()
        {
            _instance = this;
        }

        protected override void OnDispose()
        {
            _instance = null;
        }

        private void Update()
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= Time.deltaTime;
            }
            else
            {
                _timeLeft = EXECUTION_FREQUENCY;
                ProcessCharacters();
            }
        }

        private void ProcessCharacters()
        {
            for (var index = 0; index < _simulatedCharacters.Count; index++)
            {
                var character = _simulatedCharacters[index];
                character.ProcessHate();
            }
        }

        public static void AddSimulatedCharacter(CharacterMemoryHandler entry)
        {
            if (_instance == null) return;
            if (_instance._simulatedCharacters.Contains(entry) == false)
            {
                _instance._simulatedCharacters.Add(entry);
            }
        }
        
        public static void RemoveSimulatedCharacter(CharacterMemoryHandler entry)
        {
            if (_instance == null) return;
            if (_instance._simulatedCharacters.Contains(entry))
            {
                _instance._simulatedCharacters.Remove(entry);
            }
        }
    }
}

