using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    [Serializable]
    public class SimpleAnimation
    {
        public List<string> ClipNames;
        private Animation _animation;
        
        public void Init(Animation animation)
        {
            _animation = animation;
        }

        public void PlayAnimation(string animationName)
        {
            _animation.Play(animationName);
        }
    }
}

