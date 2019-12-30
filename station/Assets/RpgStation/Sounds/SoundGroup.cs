using UnityEngine;
using System.Collections.Generic;


namespace Station
{
	public class SoundGroup : MonoBehaviour 
	{
		#region FIELDS

		[SerializeField] private AudioSource _fallbackAudioSource = null;
		[Range(-1,100)]public int PoolSize = -1;
		public SoundConfig[] Sounds;
		private Dictionary<SoundConfig, AudioSourcePool> _soundPoolDic = null;
		#endregion
		
		public void Initialize(int standardPoolSize)
		{
			if (PoolSize == -1) PoolSize = standardPoolSize;

			InitializeAudioSourcePools ();
		}

		private void InitializeAudioSourcePools()
		{
			if (_fallbackAudioSource == null)
			{
				Debug.LogError("The sound group prefab "+name+" is missing a fall back audio source");
				return;
			}
            _soundPoolDic = new Dictionary<SoundConfig, AudioSourcePool> (Sounds.Length);

			var standardAudioSourcePool = new AudioSourcePool (_fallbackAudioSource, PoolSize, transform);

            for (var i = 0; i < Sounds.Length; i++)
            {
                var sound = Sounds[i];
                if (sound.Source != null)
                {
                    _soundPoolDic.Add(sound, new AudioSourcePool(sound.Source, sound.PoolSize, transform));
                }
                else
                {
                    _soundPoolDic.Add(sound, standardAudioSourcePool);
                }
            }
		}

		public Sound GetSound(string soundName)
		{
			Sound result = null;

			var soundData = GetSoundInfo(soundName);

			if (soundData != null) 
			{
				var audioSource = _soundPoolDic [soundData].Spawn ();
				result = new Sound(soundData, audioSource, _soundPoolDic [soundData]);
			}

			return result;
		}

		private SoundConfig GetSoundInfo(string soundName)
		{
			SoundConfig result = null;
            for (var i = 0; i < Sounds.Length; i++)
            {
                if (Sounds[i].name == soundName)
                {
                    result = Sounds[i];
                    break;
                }
            }

			return result;
		}
	}

	public class AudioSourcePool
	{
		private readonly AudioSource _prefab;
		private readonly List<AudioSource> _pool;
		private readonly int _size;
		private readonly Transform _parent;
		
		public AudioSourcePool (AudioSource targetPrefab, int poolSize, Transform parent)
		{
			_prefab = targetPrefab;
			_size = poolSize;
			_parent = parent;
			
			_pool = new List<AudioSource> ();
			for (var i = 0; i < _size; ++i) 
			{
				var audioSource = Object.Instantiate(_prefab);
				audioSource.transform.parent = parent;
				_pool.Add(audioSource);
			}
		}
		
		public AudioSource Spawn ()
		{
			if(_pool.Count > 0)
			{
				var audioSrc = _pool[0];
				_pool.RemoveAt(0);
				
				return audioSrc;
			}
			
			var audioSource = Object.Instantiate (_prefab);
			audioSource.transform.parent = _parent;
			
			return audioSource;
		}
		
		public void Despawn (AudioSource audioSrc)
		{
			_pool.Add(audioSrc);
		}
	}
}

