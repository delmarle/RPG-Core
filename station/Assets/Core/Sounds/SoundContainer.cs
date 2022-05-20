using System;
using UnityEngine;

namespace Station
{
    [Serializable]
    public class SoundContainer: ScriptableObject
    {
        [Serializable] public class soundsDictionary : SerializableDictionary<string, SoundHolder> {}
        public soundsDictionary Dict = new soundsDictionary();

        public SoundHolder AddSound(SoundConfig cfg)
        {
            var guid = Guid.NewGuid().ToString();
            var soundsHolder = new SoundHolder{KeyId = guid, Config = cfg};
            Dict.Add(guid,soundsHolder);
            return soundsHolder;
        }
    }
    
    [Serializable]
    public class SoundHolder
    {
        public string KeyId;
        public SoundConfig Config;
    }


}
