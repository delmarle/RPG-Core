
using UnityEngine;

namespace Station
{
    public class AmbientSoundTrigger : MonoBehaviour
    {
        [SerializeField] private bool _playOnStart = false;

        [SerializeField] private SoundConfig _sound = null;


        private int _soundId = -1;

        private void Start()
        {
            if(_playOnStart) TriggerSound();
        }

        public void TriggerSound()
        {
            _soundId = AmbientScenePlayer.PlayAmbientSound(_sound.name);
        }

        public void StopAmbientSound()
        {
            if (_soundId != -1)
            {
                AmbientScenePlayer.StopAmbientSound(_soundId); 
            }
        }

    }
}
