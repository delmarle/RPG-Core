using UnityEngine;


namespace Station
{

    [CreateAssetMenu(fileName = "SourcePoolConfig", menuName = "Tools/Audio/SourcePoolConfig", order = 1)]
    public class SourcePoolConfig : ScriptableObject
    {
        public AudioSource FallbackSource = null;
        [Range(-1, 100)] public int PoolSize = -1;
    }
}