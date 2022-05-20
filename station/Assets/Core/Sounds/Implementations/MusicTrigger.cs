
using UnityEngine;

namespace Station
{
    public class MusicTrigger : MonoBehaviour
    {
        [SerializeField] private bool _playOnStart = false;

        [SerializeField] private SoundConfig _sound = null;

    

        private void Start()
        {
            if(_playOnStart) TriggerSound();
        }

        public void TriggerSound()
        {
            MusicSoundPlayer.PlayMusic(_sound.name);
        }
    }
}
