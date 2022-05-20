using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class SoundSystem : BaseSystem
    {
        #region FIELDS
        private static SoundSystem _instance;
        private SoundPlayer _soundPlayer;
        private SoundsDb _soundsDb;
        private FootstepsDb _footStepsDb;
        #endregion
        
        protected override void OnInit()
        {
            _instance = this;
        }



        protected override void OnDispose()
        {
            _instance = null;
        }

        protected override void OnDataBaseReady()
        {
            _soundsDb = GameInstance.GetDb<SoundsDb>();
            _footStepsDb = GameInstance.GetDb<FootstepsDb>();
            _soundPlayer = gameObject.GetComponentInChildren<SoundPlayer>();
            _soundPlayer.Initialize();
            HashSet<SoundConfig> sounds = new HashSet<SoundConfig>();
            foreach (var soundContainer in _soundsDb.PersistentContainers)
            {
                foreach (var sound in soundContainer.Dict.Values)
                {
                    sounds.Add(sound.Config);
                }
            }

            foreach (var template in _footStepsDb)
            {
                foreach (var entry in template.Entries)
                {
                    sounds.Add(entry.Sounds);
                }
            }
            _soundPlayer.InjectSounds(sounds);
        }

        #region CONTOLS
        public int PlaySound(string soundId)
        {
            return _soundPlayer.Play(soundId);
        }
        
        public static void PlayFootStep(string name,Transform character)
        {
            if (_instance)
            {
                _instance._soundPlayer.Play(name, character);
            }
            else
            {
                Debug.LogWarning("FootStepsPlayer is missing !");
            }
        }
        
        #endregion
    }
}
