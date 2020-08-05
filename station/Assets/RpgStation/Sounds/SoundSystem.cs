using System.Collections.Generic;

namespace Station
{
    public class SoundSystem : BaseSystem
    {
        //cached components
        
        private SoundPlayer _soundPlayer;
        private SoundsDb _soundsDb;
        
        
        protected override void OnInit()
        {
            GameGlobalEvents.OnDataBaseLoaded.AddListener(OnDbReady);
        }



        protected override void OnDispose()
        {
            GameGlobalEvents.OnDataBaseLoaded.RemoveListener(OnDbReady);
        }
        
        private void OnDbReady()
        {
            var dbSystem = RpgStation.GetSystemStatic<DbSystem>();
            _soundsDb = dbSystem.GetDb<SoundsDb>();
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
            _soundPlayer.InjectSounds(sounds);
        }

        #region CONTOLS
        public void PlaySound(string soundId)
        {
        }
        #endregion
    }
}
