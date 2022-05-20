using System;
using UnityEngine;

namespace Station
{
    public class SoundEvent
        {
            #region FIELDS
            public delegate void SoundEventHandler (SoundEvent soundEvent);
            public event SoundEventHandler OnComplete;
            
            private string _name;
            private int _eventId;
            private Transform _attachPoint;
            private Vector3 _position;
            private Sound _sound;
            private readonly SoundFader _fader;
            private bool _isDelaying;
            private float _startTime;
            
            public string Name{ get { return _name; } }
            public int EventId { get{ return _eventId; } }
            public Sound Sound{get{ return _sound; }}

			public float TargetVolume{get; private set;}
            #endregion
            public SoundEvent ()
            {
                _eventId = -1;
                _sound = null;
                _fader = new SoundFader ();
            }
            
            public void Start (int id, Sound aSound, Transform parent = null, Vector3 position = new Vector3(), bool mute = false)
            {
                _name = aSound.SoundConfig.name;
                _eventId = id;
                _attachPoint = parent;
                _position = position;
                _sound = aSound;
				TargetVolume = _sound.SoundConfig.Volume;

                InitializeSource();
                ToggleMute(mute);

                if (_sound.SoundConfig.DelayAtStart <= 0)
                {
                    _sound.AudioSource.Play();
                }
                else
                {
                    _isDelaying = true;
                    _startTime = Time.realtimeSinceStartup;
                }
                
                _fader.FadeIn(_sound.AudioSource, _sound.AudioSource.volume, _sound.SoundConfig.FadeInTime);
            }

            private void InitializeSource()
            {
                var audioSource = _sound.AudioSource;
                var soundData = _sound.SoundConfig;
                
                audioSource.transform.position = _position;
                audioSource.clip = soundData.RandomizedClip ();
                audioSource.volume = soundData.Volume;
                audioSource.loop = soundData.Looping;
                audioSource.pitch = soundData.RandomizedPitch();
            }

			public void Mute(float fadeout = -1f)
			{
				_fader.FadeOut(Math.Abs(fadeout - (-1)) < 0.01f ? 0f : fadeout);
				if (Math.Abs(fadeout - (-1)) < 0.01f)
				{
					_fader.FadeOut(0f);
				}
				else
				{
					_fader.FadeOut(fadeout);
				}
			}

			public void UnMute(float fadein = -1f)
			{
				_fader.FadeIn(_sound.AudioSource, TargetVolume, Math.Abs(fadein - (-1)) < 0.01f ? 0f : fadein, GetVolume());
			}
            
            public void Stop (float fadeout = -1f)
            {
				_fader.OnFadeComplete += OnFaderComplete;

                if (Math.Abs(fadeout - (-1)) < 0.01f)
                {
                    _fader.FadeOut(_sound.SoundConfig.FadeOutTime);
                }
                else
                {
                    _fader.FadeOut(fadeout);
                }
            }
            
            public void SetPosition (Vector3 position)
            {
                _sound.AudioSource.transform.position = position;
            }
            
            public void SetVolume (float volume, bool stopFade)
            {
                if(stopFade)
                {
                    _fader.Stop();
					_sound.AudioSource.volume = volume * SoundPlayer.RelativeVolume;
                }
            }

            public float GetVolume()
            {
            	return _sound.AudioSource.volume;
            }

			public bool IsFadingIn()
			{
				return _fader.CurrentState == SoundFader.State.FadeIn;
			}

			public bool IsFadingOut()
			{
				return _fader.CurrentState == SoundFader.State.FadeOut;
			}
            
            public void SetPitch (float pitch)
            {
                _sound.AudioSource.pitch = pitch;
            }
            
            public void ToggleMute(bool mute)
            {
                _sound.AudioSource.mute = mute;
            }

            public void SetTimePosition(float percent)
            {
                var audioSource = _sound.AudioSource;
                audioSource.time = percent * audioSource.clip.length;
            }

            public float GetTimePosition()
            {
                var audioSource = _sound.AudioSource;
                return audioSource.time / audioSource.clip.length;
            }
            
            public void Update ()
            {
                if (_isDelaying)
                {
                    if (Time.realtimeSinceStartup - _startTime >= _sound.SoundConfig.DelayAtStart)
                    {
                        _isDelaying = false;
                        _sound.AudioSource.Play();
                    }
                }
                else
                {
                    if (_sound.AudioSource != null)
                    {
                        if (!_sound.AudioSource.loop && !_sound.AudioSource.isPlaying && !_isDelaying)
                        {
                            _fader.Stop();
                            _attachPoint = null;
                            DoOnComplete();
                        }
                    }
                    
                    _fader.Update();
                    
                    if (_attachPoint != null)
                    {
                        if (_sound.AudioSource != null) _sound.AudioSource.transform.position = _attachPoint.position;
                    }
                }
            }

            private void OnFaderComplete (SoundFader.State state)
            {
				_fader.OnFadeComplete -= OnFaderComplete;

                if(state == SoundFader.State.FadeOut)
                {
                    _attachPoint = null;
                    DoOnComplete();
                }
            }

            private void DoOnComplete ()
            {
                if(OnComplete != null)
                {
                    OnComplete(this);
                }
            }
        }

       
}