using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [Serializable]
    public class ClipsAnimation: BaseAnimation
    {
        [SerializeField] public List<StateData> AnimationModels = new List<StateData>();

        public bool UseDefaultState;
        public string DefaultState;
        private Animation _animation;
        private Dictionary<string, List<ClipData>> _cacheData;
        private Dictionary<int, AnimationClip> _layerStatus;
        public string CurrentState => _currentState;
        private bool _isFilled = false;
        private string _currentState;

        private void Awake()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            if(AnimationModels == null)
                AnimationModels = new List<StateData>();

            if (_isFilled)
            {
                return;
            }

            _animation = GetComponent<Animation>();
            if (_animation == null)
            {
                _animation = gameObject.AddComponent<Animation>();
            }

            _cacheData = new Dictionary<string, List<ClipData>>();
            _layerStatus = new Dictionary<int, AnimationClip>();
            ClearAllAnim();
            foreach (var animationModel in AnimationModels)
            {
                _cacheData[animationModel.State] = animationModel.AnimationClipModels;
                foreach (var clip in animationModel.AnimationClipModels)
                {
                    if (clip.Clip == null)
                    {
                       Debug.LogError("missing animation clip");
                        continue;
                    }

                    clip.Clip.legacy = true;
                    _animation.AddClip(clip.Clip, clip.Clip.name);
                }
            }

            if (UseDefaultState && _cacheData.ContainsKey(DefaultState) == false) 
            {
                Debug.LogError("default state not found");
            }
            _isFilled = true;
        }
       
        private void ClearAllAnim()
        {
            _animation.clip = null;

            var animationNames = (from AnimationState animationState in _animation select animationState.clip.name).ToList();

            for (var index = animationNames.Count - 1; index >= 0; index--)
            {
                var animationName = animationNames[index];
                _animation.RemoveClip(animationName);
            }
        }

        public void OnEnable()
        {
            if (_animation)
            {
                _animation.enabled = true;

                if (UseDefaultState)
                {
                    PlayState(DefaultState);
                }
            }
        }

        public override void PlayState(string stateName, bool checkIsActiveInHierarchy = false, bool forcePlay = false)
        {
            if (_cacheData == null)
            {
                Initialize();
            }

            if (_cacheData == null)
            {
                return;
            }

            if (checkIsActiveInHierarchy)
            {
                if (!_animation.gameObject.activeInHierarchy)
                {
                    return;
                }
            }
           
            _currentState = stateName;
            if (!_cacheData.ContainsKey(stateName))
            {
                Debug.LogError("missing animation state");
                return;
            }
            var animationClipModels = _cacheData[stateName];
            foreach (var clipModel in animationClipModels)
            {
                int animIndex = clipModel.LayerIndex;
                AnimationClip animationClip = null;
                if (_layerStatus.ContainsKey(animIndex))
                {
                    animationClip = _layerStatus[animIndex];
                }

                if (animationClip != null)
                {
                    AnimationState animationState = _animation[animationClip.name];
                    if (animationState)
                    {
                        animationState.enabled = false;
                    }
                    else
                    {
                       Debug.LogWarning("clip should be legacy");
                        return;
                    }
                   
                }

                if (clipModel.Clip != null)
                {
                    if (_animation.isPlaying)
                    {
                        _animation.Blend(clipModel.Clip.name, clipModel.Weight, 0.5f);
                        _animation.Sample();
                    }
                    else
                    {
                        _animation.Play(clipModel.Clip.name);
                        _animation.Sample();
                    }
                }

                _layerStatus[clipModel.LayerIndex] = clipModel.Clip;
            }
        }

        public override float GetStateDuration(string stateName)
        {
            float duration = 0;
            if (_cacheData == null || !_cacheData.ContainsKey(stateName))
            {
                Debug.LogError("animation state not found");
                return 0;
            }
            var animationClipModels = _cacheData[stateName];
           
            foreach (ClipData clipModel in animationClipModels)
            {
                if (clipModel.Clip == null)
                {
                    continue;
                }
                if (Math.Abs(duration) < 0.01f || duration < clipModel.Clip.length)
                {
                    duration = clipModel.Clip.length;
                }
            }

            return duration;
        }

        public override void StopAllAnimations()
        {
            if (_animation)
            {
                _animation.Stop();
            }
        }
    }
    
    [Serializable]
    public class StateData
    {
        public string State;
        public List<ClipData> AnimationClipModels = new List<ClipData>();
    }
   
    [Serializable]
    public class ClipData
    {
        public AnimationClip Clip;
        public float Weight = 0.5f;
        public int LayerIndex;
    }
}

