using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
  public class SurfaceSceneReference : MonoBehaviour
    {
        public List<SurfaceSceneEntry> Entries = new List<SurfaceSceneEntry>();
        
        //caching
        private Dictionary<Texture, string> _surfacesMap = new Dictionary<Texture, string>();

        private void Awake()
        {
            foreach (var entry in Entries)
            {
                foreach (var texture in entry.Textures)
                {
                    if (texture != null)
                    {
                        if (_surfacesMap.ContainsKey(texture) == false)
                        {
                            _surfacesMap.Add(texture, entry.SurfaceName);
                        }
                    }
                }
            }
        }

        public string ResolveTextureAsSurface(Texture texture)
        {
            if (_surfacesMap.ContainsKey(texture))
            {
                return _surfacesMap[texture];
            }

            return "default";
        }
    }

}