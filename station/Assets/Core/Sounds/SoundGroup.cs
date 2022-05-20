
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;


namespace Station
{


	public class SoundGroup : MonoBehaviour 
	{
		#region FIELDS

		[SerializeField] private SourcePoolConfig _fallbackAudioSource = null;

		private SoundConfig[] Sounds;
		private Dictionary<AudioSource, AudioSourcePool> _soundPoolDic = null;
		#endregion
		
		public void Initialize(int standardPoolSize)
		{
			if (_fallbackAudioSource.PoolSize == -1) _fallbackAudioSource.PoolSize = standardPoolSize;
			
			InitializeAudioSourcePools ();
		}

		private void InitializeAudioSourcePools()
		{
		
		}

		public void InjectSounds(SoundConfig[] sounds)
		{
			Sounds = sounds;
			if (_fallbackAudioSource == null)
			{
				Debug.LogError("The sound group prefab "+name+" is missing a fall back audio source");
				return;
			}
			_soundPoolDic = new Dictionary<AudioSource, AudioSourcePool> (Sounds.Length);
			var standardAudioSourcePool = new AudioSourcePool (_fallbackAudioSource, transform);
			_soundPoolDic.Add(_fallbackAudioSource.FallbackSource, standardAudioSourcePool);
			for (var i = 0; i < Sounds.Length; i++)
			{
				var sound = Sounds[i];
				var source = sound?.SourceConfig?.FallbackSource;
				if (source == null)
				{
					source = _fallbackAudioSource.FallbackSource;
				}

				if (source != null)
				{
					if ( _soundPoolDic.ContainsKey(source) == false)
					{
						_soundPoolDic.Add(source, new AudioSourcePool(sound.SourceConfig, transform));
					}
				}
			}
			
		}

		public Sound GetSound(string soundName)
		{
			Sound result = null;

			var soundData = GetSoundInfo(soundName);

			if (soundData != null)
			{

				var audioSourceKey = soundData.SourceConfig?.FallbackSource;
				if (audioSourceKey == null)
				{
					audioSourceKey = _fallbackAudioSource.FallbackSource;
				}

				var audioSource = _soundPoolDic [audioSourceKey].Spawn ();
				result = new Sound(soundData, audioSource, _soundPoolDic [audioSourceKey]);
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
		private readonly Transform _parent;
		
		public AudioSourcePool (SourcePoolConfig data, Transform parent)
		{
			_prefab = data.FallbackSource;
			_parent = parent;
			
			_pool = new List<AudioSource> ();
			for (var i = 0; i < data.PoolSize; ++i) 
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

