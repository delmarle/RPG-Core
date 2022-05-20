using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public abstract class BaseAnimation : MonoBehaviour
    {
        private void Awake()
        {
            Initialize();
        }

        protected abstract void Initialize();
        public abstract void PlayState(string stateName, bool checkIsActiveInHierarchy = false, bool forcePlay = true);
        public abstract float GetStateDuration(string stateName);
        public abstract void StopAllAnimations();
    }
}

