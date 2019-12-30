
using UnityEngine;

namespace Station
{
    public class EffectSoundTrigger : MonoBehaviour
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
            _soundId = EffectSoundPlayer.PlayEffectSound(_sound.name);
        }

        public void StopEffectSound()
        {
            if(_soundId != -1)
                EffectSoundPlayer.StopEffectSound(_soundId); 
        }

    }
}
