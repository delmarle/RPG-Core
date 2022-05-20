namespace Station
{
    public class AmbientScenePlayer : SoundPlayer
    {
        private static AmbientScenePlayer _instance;

        public override void Initialize()
        {
            _instance = this;
            base.Initialize();
        }

        public static int PlayAmbientSound(string soundName)
        {
            if (_instance == null) return -1;
            
            return _instance.Play(soundName);
        }

        public static void StopAmbientSound(int soundId)
        {
            if (_instance == null) return;
            
            _instance.StopSound(soundId);
        }
        
        public static void StopAllAmbientSounds()
        {
            if (_instance == null) return;
            
            _instance.StopLocalSounds();
        }
    }
}


