using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Footsteps")]
    public class FootstepsDb : DictGenericDatabase<FootSoundTemplate>
    {
        public List<string> Surfaces = new List<string>();
        public SourcePoolConfig DefaultFootstepConfig;
        
        public List<SoundContainer> PersistentContainers = new List<SoundContainer>();
        [Serializable] public class ElementDictionary : SerializableDictionary<string, FootSoundTemplate> {}
        [SerializeField] private ElementDictionary _db = new ElementDictionary();
    
        public override IDictionary<string, FootSoundTemplate> Db
        {
            get => _db;
            set => _db.CopyFrom (value);
        }
        
        public override string ObjectName()
        {
            return "Sounds";
        }
        
        public override string[] ListEntryNames()
        {
            return _db.Select(entry => entry.Value.name).ToArray();
        }
    }



    [Serializable]
    public class SurfaceEntry
    {
        public string SurfaceName = "Surface";
        public SoundConfig Sounds;
    }

    [Serializable]
    public class SurfaceSceneEntry
    {
        public string SurfaceName = "Surface";
        public List<Texture> Textures = new List<Texture>();
    }
  
}

