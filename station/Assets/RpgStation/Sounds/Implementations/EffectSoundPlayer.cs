namespace Station
{
    public class EffectSoundPlayer : SoundPlayer
    {
        private static EffectSoundPlayer _instance;

        public override void Initialize()
        {
            _instance = this;
            base.Initialize();
        }

        public static int PlayEffectSound(string soundName)
        {
            if (_instance == null) return -1;
            
            return _instance.Play(soundName);
        }

        public static void StopEffectSound(int soundId)
        {
            if (_instance == null) return;
            
            _instance.StopSound(soundId);
        }
        
        public static void StopAllEffectSounds()
        {
            if (_instance == null) return;
            
            _instance.StopLocalSounds();
        }
    }
}


