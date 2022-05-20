using UnityEngine;

namespace Station
{
    public class SoundFader 
        {

            public enum State
            {
                None,
                FadeIn,
                FadeOut
            }
            
            public delegate void SoundFadeHandler (State state);
            public event SoundFadeHandler OnFadeComplete;
            
            private AudioSource _targetAudioSource;
            
            private float _fadeSpeed;
            private float _targetVolume;
            private float _volumeDiff;
            
            private bool _doFade;
            
			public State CurrentState{get{return _curState;}}
			private State _curState;
            
            public SoundFader ()
            {
                _targetAudioSource = null;
                _fadeSpeed = 0;
                _targetVolume = 0;
                
                _curState = State.None;
            }
            
			public void FadeIn (AudioSource audioSrc, float targetVol, float duration, float initialVolume = 0f)
            {
                _curState = State.FadeIn;
                _targetAudioSource = audioSrc;
				_targetAudioSource.volume = initialVolume;
                InitFade (targetVol, duration);
            }
            
            public void FadeOut (float duration)
            {
                _curState = State.FadeOut;
                InitFade (0, duration);
            }
            
            public void Stop ()
            {
				_doFade = false;
            }
            
            public void Update ()
            {
                if (_doFade) 
                {
                    _volumeDiff = Mathf.MoveTowards(_volumeDiff, 0, Time.deltaTime * _fadeSpeed);
                    if(Mathf.Abs(_volumeDiff) <= 0.001f)
                    {
                        _volumeDiff = 0;
                        _targetAudioSource.volume = _targetVolume * SoundPlayer.RelativeVolume;
						_doFade = false;

                        CallOnComplete();
                    }
					else
					{
                    	_targetAudioSource.volume = (_targetVolume - _volumeDiff) * SoundPlayer.RelativeVolume;
					}
                }
            }
            
            private void InitFade (float targetVol, float duration)
            {
                if (duration <= 0.001f) 
                {
                    _doFade = false;
                    _targetAudioSource.volume = targetVol * SoundPlayer.RelativeVolume;
                    
                    CallOnComplete();
                }
                else
                {
                    _doFade = true;
                    _targetVolume = targetVol;
                    _volumeDiff = _targetVolume - _targetAudioSource.volume;
                    
                    _fadeSpeed = Mathf.Abs(_volumeDiff / duration);
                }
            }
            
            private void CallOnComplete ()
            {
                if(OnFadeComplete != null)
                {
                    OnFadeComplete(_curState);
                }
            }
        }
}
