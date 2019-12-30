using System;
using Station;


namespace Station
{
    public class CoolDown
    {
        private Timer _timer;

        public bool IsReady()
        {
            if (_timer == null)
            {
                return true;
            }

            return _timer.IsCompleted;
        }

        public void TriggerCoolDown(float time, Action OnReady)
        {
            _timer = Timer.Register(time, OnReady.Invoke);
        }

        public float TimeLeft
        {
            get { return _timer != null ? _timer.GetTimeRemaining() : 0; }
        }
    }
}

