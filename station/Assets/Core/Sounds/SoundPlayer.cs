using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Station
{
	public class SoundPlayer : MonoBehaviour 
	{
		#region FIELDS
		protected bool Initialized;
		
		protected delegate void SoundPlayerHandler (bool mute);

		[SerializeField] private SoundGroup _persistentSoundGroup = null;
		[SerializeField] private SoundGroup _temporarySoundGroup = null;
		[SerializeField] private int _soundEventPoolSize = 10;
        [SerializeField] private bool _autoInitialize = true;

		private SoundEventPool _soundEventPool;
        private List<int> _curPlayingTraceableSoundIds;
        private List<SoundEvent> _curPlayingTraceableSounds;
		private List<int> _soundEventsToStop;
		private IdGenerator _eventIdGenerator;

		protected bool Mute;
		#endregion
		#region Static	
		protected static event SoundPlayerHandler OnSoundsMute;
		
		private static bool _sSoundMute;
		
		private static float _relativeVolume = 1.0f;
		public Action<int> OnSoundComplete;
		
		public static float RelativeVolume 
		{ 
			get { return _relativeVolume; }
			set
			{ 
				_relativeVolume = value; 
				_relativeVolume = Mathf.Clamp01(_relativeVolume);
			} 
		}
		
		public static void ToggleSoundsMute (bool mute)
		{
			_sSoundMute = mute;
			
			if (OnSoundsMute != null)
				OnSoundsMute (mute);
		}
		
		
		#endregion
		
		#region Sounds related

        public int Play(string soundname)
        {
            return Play(soundname, null, default(Vector3)); 
        }

        public int Play(string soundname, Transform parent)
        {
            return Play(soundname, parent, default(Vector3));
        }

        public int Play(string soundname, Vector3 position)
        {
            return Play(soundname, null, position);
        }

		public void StopSound (int soundId, float fadeout = -1f)
		{
			if(CurrentlyPlayingSoundEvent(soundId))
			{
				if(!_soundEventsToStop.Contains(soundId))
				{
					var playSoundEvent = GetSoundEventFromId(soundId);
					playSoundEvent.Stop(fadeout);
				}
			}
		}

		public void StopSound (string soundname, float fadeout = -1f)
		{
            for (var i = 0; i < _curPlayingTraceableSoundIds.Count; i++)
            {
                var soundEventId = _curPlayingTraceableSoundIds[i];
                if (!_soundEventsToStop.Contains(soundEventId))
                {
                    if (_curPlayingTraceableSounds[i].Name == soundname)
                    {
                        _curPlayingTraceableSounds[i].Stop(fadeout);
                    }
                }
            }
		}

		public void MuteSound(string soundname, float fadeout = -1f)
		{
			for (var i = 0; i < _curPlayingTraceableSoundIds.Count; i++)
			{
				var soundEventId = _curPlayingTraceableSoundIds[i];
				if (!_soundEventsToStop.Contains(soundEventId))
				{
					if (_curPlayingTraceableSounds[i].Name == soundname)
					{
						_curPlayingTraceableSounds[i].Mute(fadeout);
					}
				}
			}
		}

		public void MuteSound (int soundId, float fadeout = -1f)
		{
			if(CurrentlyPlayingSoundEvent(soundId))
			{
				if(!_soundEventsToStop.Contains(soundId))
				{
					var playSoundEvent = GetSoundEventFromId(soundId);
					playSoundEvent.Mute(fadeout);
				}
			}
		}

		public void UnMuteSound(string soundname, float fadein = -1f)
		{
			for (var i = 0; i < _curPlayingTraceableSoundIds.Count; i++)
			{
				var soundEventId = _curPlayingTraceableSoundIds[i];
				if (!_soundEventsToStop.Contains(soundEventId))
				{
					if (_curPlayingTraceableSounds[i].Name == soundname)
					{
						_curPlayingTraceableSounds[i].UnMute(fadein);
					}
				}
			}
		}

		public void UnMuteSound (int soundId, float fadein = -1f)
		{
			if(CurrentlyPlayingSoundEvent(soundId))
			{
				if(!_soundEventsToStop.Contains(soundId))
				{
					var playSoundEvent = GetSoundEventFromId(soundId);
					playSoundEvent.UnMute(fadein);
				}
			}
		}

		public void StopLocalSounds ()
		{
            for (var i = 0; i < _curPlayingTraceableSoundIds.Count; i++)
            {
                var soundEventId = _curPlayingTraceableSoundIds[i];
                if (!_soundEventsToStop.Contains(soundEventId))
                {
                    _curPlayingTraceableSounds[i].Stop();
                }
            }
		}
		
		public bool IsSoundPlaying (int soundId)
		{
			return CurrentlyPlayingSoundEvent(soundId);
		}
		
		public bool IsSoundPlaying (string soundname)
		{
            for (var i = 0; i < _curPlayingTraceableSounds.Count; i++)
            {
                if (_curPlayingTraceableSounds[i].Name == soundname) return true;
            }
			return false;
		}

		public void SetVolume (int soundId, float volume, bool stopFade = false)
		{
			if (!CurrentlyPlayingSoundEvent(soundId)) return;
			
			var playSoundEvent = GetSoundEventFromId(soundId);
			playSoundEvent.SetVolume(volume, stopFade);
		}

		public float GetVolume(int soundId)
		{
			var result = -1f;
			if(CurrentlyPlayingSoundEvent(soundId))
			{
				var playSoundEvent = GetSoundEventFromId(soundId);
				result = playSoundEvent.GetVolume();
			}
			return result;
		}

		public bool IsInstanceSoundFadingIn(int soundId)
		{
			var result = false;

			if(CurrentlyPlayingSoundEvent(soundId))
			{
				var playSoundEvent = GetSoundEventFromId(soundId);
				result = playSoundEvent.IsFadingIn();
			}
			return result;
		}

		public bool IsInstanceSoundFadingOut(int soundId)
		{
			var result = false;

			if(CurrentlyPlayingSoundEvent(soundId))
			{
				var playSoundEvent = GetSoundEventFromId(soundId);
				result = playSoundEvent.IsFadingOut();
			}
			return result;
		}

		public float GetTargetVolume(int soundId)
		{
			var result = -1f;

			if(CurrentlyPlayingSoundEvent(soundId))
			{
				var playSoundEvent = GetSoundEventFromId(soundId);
				result = playSoundEvent.TargetVolume;
			}

			return result;
		}

		public void SetPitch (int soundId, float pitch)
		{
			if(CurrentlyPlayingSoundEvent(soundId))
			{
                var playSoundEvent = GetSoundEventFromId(soundId);
                playSoundEvent.SetPitch(pitch);
			}
		}

		public void SetPlayTime (int soundId, float normalizedTime)
		{
			if(CurrentlyPlayingSoundEvent(soundId))
			{
                var playSoundEvent = GetSoundEventFromId(soundId);
                playSoundEvent.SetTimePosition(normalizedTime);
			}
		}

		public float GetPlayTime (int soundId)
		{
			var result = -1f;

			if(CurrentlyPlayingSoundEvent(soundId))
			{
                var playSoundEvent = GetSoundEventFromId(soundId);
                result = playSoundEvent.GetTimePosition();
			}

			return result;
		}

		public void SetVolumeForSoundPlayer(float volume, bool stopFade = false)
		{
			for(var i=0;i<_curPlayingTraceableSounds.Count;i++)
			{
				_curPlayingTraceableSounds[i].SetVolume(volume, stopFade);
			}
		}

		#endregion

		public void InjectSounds(HashSet<SoundConfig> sounds)
		{
			_persistentSoundGroup.InjectSounds(sounds.ToArray());
		}

		public virtual void Initialize()
		{
			if(!Initialized)
			{
				Initialized = true;

				Mute = _sSoundMute;
				OnSoundsMute += OnMuteStateChange;

				_soundEventPool = new SoundEventPool(_soundEventPoolSize);

				if(_persistentSoundGroup.transform.parent != transform)
				{
					_persistentSoundGroup = Instantiate(_persistentSoundGroup, Vector3.zero, Quaternion.identity);
					_persistentSoundGroup.transform.parent = transform;
				}

				_persistentSoundGroup.Initialize(_soundEventPoolSize);

				_curPlayingTraceableSoundIds = new List<int>(_soundEventPoolSize);
				_curPlayingTraceableSounds = new List<SoundEvent>(_soundEventPoolSize);

				_soundEventsToStop = new List<int> (_soundEventPoolSize);
				_eventIdGenerator = new IdGenerator ();
			}
		}
		
		#region Monobehaviour
		private void Awake () 
		{
            if (_autoInitialize)
            {
                Initialize();
            }
		}

		protected virtual void OnDestroy ()
		{
			OnSoundsMute -= OnMuteStateChange;
			
		}
		
		void Update ()
		{
            if (Initialized)
            {
                UpdateSoundEvents();
            }
		}
		
		void LateUpdate ()
		{
            if (Initialized)
            {
                CleanCompletedSoundEvents();
            }
		}
		#endregion
		
        private void UpdateSoundEvents()
        {
	        if (_curPlayingTraceableSounds == null) return;
            for (var i = 0; i < _curPlayingTraceableSounds.Count; i++)
            {
                _curPlayingTraceableSounds[i].Update();
            }
        }

        private void CleanCompletedSoundEvents()
        {
            for(var i = 0; i < _soundEventsToStop.Count; i++)
            {
                var soundEventId = _soundEventsToStop[i];
                var playSoundEvent = GetSoundEventFromId(soundEventId);

                playSoundEvent.Sound.DespawnAudioSource();
                _soundEventPool.Despawn(playSoundEvent);

                RemoveSoundFromTraceableSounds(soundEventId);

				if(OnSoundComplete != null)
				{
					OnSoundComplete(soundEventId);
				}
            }
            
            if(_soundEventsToStop.Count > 0)
            {
                _soundEventsToStop.Clear();
            }
        }

        private void AddSoundToTraceableSounds(int soundEventId, SoundEvent soundEvent)
        {
            _curPlayingTraceableSoundIds.Add(soundEventId);
            _curPlayingTraceableSounds.Add(soundEvent);
        }

        private void RemoveSoundFromTraceableSounds(int soundEventId)
        {
            var findId = _curPlayingTraceableSoundIds.FindIndex(e => e == soundEventId);
            if (findId != -1)
            {
                _curPlayingTraceableSounds.RemoveAt(findId);
                _curPlayingTraceableSoundIds.RemoveAt(findId);
            }
        }

        private bool CurrentlyPlayingSoundEvent(int soundEventId)
        {
            return _curPlayingTraceableSoundIds.FindIndex(e => e == soundEventId) != -1;
        }

        private SoundEvent GetSoundEventFromId(int soundEventId)
        {
            var findId = _curPlayingTraceableSoundIds.FindIndex(e => e == soundEventId);
            return findId != -1 ? _curPlayingTraceableSounds[findId] : null;
        }
		
		private int Play (string soundname, Transform targetParent, Vector3 targetPosition)
		{
			var soundEventId = -1;
			Sound sound = null;

			sound = _persistentSoundGroup.GetSound(soundname);

			if(sound != null)
			{
				soundEventId = StartSoundEvent(sound, targetParent, targetPosition);
			}
			
			return soundEventId;
		}

		private int StartSoundEvent (Sound sound, Transform parent, Vector3 position)
		{
            var soundEventId = _eventIdGenerator.Generate();
            var playSoundEvent = _soundEventPool.Spawn();
            playSoundEvent.OnComplete += OnSoundEventComplete;

            playSoundEvent.Start(soundEventId, sound, parent, position, Mute);
            AddSoundToTraceableSounds(soundEventId, playSoundEvent);

            return soundEventId;
 		}

		private void OnSoundEventComplete (SoundEvent soundEvent)
		{
			if(_curPlayingTraceableSounds.Find(e => e.EventId == soundEvent.EventId) != null)
			{
				_soundEventsToStop.Add(soundEvent.EventId);
				soundEvent.OnComplete -= OnSoundEventComplete;
			}
		}

		protected void OnMuteStateChange (bool mute)
		{
			if(Mute != mute)
			{
                for (var i = 0; i < _curPlayingTraceableSounds.Count; i++)
                {
                    _curPlayingTraceableSounds[i].ToggleMute(mute);
                }
				
				Mute = mute;
			}
		}
	}
}

