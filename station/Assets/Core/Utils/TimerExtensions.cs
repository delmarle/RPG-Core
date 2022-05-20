using System;
using UnityEngine;

namespace Station
{
    public static class TimerExtensions
    {
        public static void AttachTimer(this MonoBehaviour behaviour, float duration, Action onComplete,
            Action<float> onUpdate = null, bool isLooped = false, bool useRealTime = false)
        {
            Timer.Register(duration, onComplete, onUpdate, isLooped, useRealTime, behaviour);
        }

        public static void SafeCancelTimer(this Timer timer)
        {
            if (timer != null && (timer.IsCompleted == false ||timer.IsCancelled == false))
            {
                timer.Cancel();
            }
        }
    }
}