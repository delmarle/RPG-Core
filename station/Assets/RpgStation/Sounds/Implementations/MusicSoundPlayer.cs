
namespace Station
{
    public class MusicSoundPlayer : SoundPlayer
    {
        private static MusicSoundPlayer _instance;
        
        private static event SoundPlayerHandler OnMusicMute;
        private static bool _sMusicdMute;
        
        public override void Initialize()
        {
            if (!Initialized)
            {
                _instance = this;
                base.Initialize();
                Mute = _sMusicdMute;
                OnSoundsMute -= OnMuteStateChange;
                OnMusicMute += OnMuteStateChange;
            }   
        }

        protected override void OnDestroy()
        {
            OnMusicMute -= OnMuteStateChange;
        }

        public static void ToggleMusicMute (bool mute)
        {
            _sMusicdMute = mute;
			
            if (OnMusicMute != null)
                OnMusicMute (mute);
        }

        public static int PlayMusic(string musicName)
        {
            if (_instance == null) return -1;
            
            _instance.StopLocalSounds();
            return _instance.Play(musicName);
        }
    }
}

