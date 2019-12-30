using UnityEngine;
using System;
using System.Collections.Generic;

namespace Station
{
    public class Timer
{
    #region Public Properties/Fields
    public float Duration { get; private set; }

    public bool IsLooped { get; set; }
    
    public bool IsCompleted { get; private set; }

    public bool UsesRealTime { get; private set; }

    public bool IsPaused
    {
        get { return _timeElapsedBeforePause.HasValue; }
    }

    public bool IsCancelled
    {
        get { return _timeElapsedBeforeCancel.HasValue; }
    }

    public bool IsDone
    {
        get { return IsCompleted || IsCancelled || IsOwnerDestroyed; }
    }

    #endregion

    #region Public Static Methods

    public static Timer Register(float duration, Action onComplete, Action<float> onUpdate = null,
        bool isLooped = false, bool useRealTime = false, MonoBehaviour autoDestroyOwner = null)
    {
        // create a manager object to update all the timers if one does not already exist.
        if (_manager == null)
        {
            TimerManager managerInScene = UnityEngine.Object.FindObjectOfType<TimerManager>();
            if (managerInScene != null)
            {
                _manager = managerInScene;
            }
            else
            {
                GameObject managerObject = new GameObject { name = "TimerManager" };
                _manager = managerObject.AddComponent<TimerManager>();
            }
        }

        Timer timer = new Timer(duration, onComplete, onUpdate, isLooped, useRealTime, autoDestroyOwner);
        _manager.RegisterTimer(timer);
        return timer;
    }

    public static void Cancel(Timer timer)
    {
        if (timer != null)
        {
            timer.Cancel();
        }
    }

    public static void Pause(Timer timer)
    {
        if (timer != null)
        {
            timer.Pause();
        }
    }

    public static void Resume(Timer timer)
    {
        if (timer != null)
        {
            timer.Resume();
        }
    }

    public static void CancelAllRegisteredTimers()
    {
        if (_manager != null)
        {
            _manager.CancelAllTimers();
        }

        // if the manager doesn't exist, we don't have any registered timers yet, so don't
        // need to do anything in this case
    }

    #endregion

    #region Public Methods
    public void Cancel()
    {
        if (IsDone)
        {
            return;
        }

        _timeElapsedBeforeCancel = GetTimeElapsed();
        _timeElapsedBeforePause = null;
    }

    public void Pause()
    {
        if (IsPaused || IsDone)
        {
            return;
        }

        _timeElapsedBeforePause = GetTimeElapsed();
    }

    public void Resume()
    {
        if (!IsPaused || IsDone)
        {
            return;
        }

        _timeElapsedBeforePause = null;
    }

    public float GetTimeElapsed()
    {
        if (IsCompleted || GetWorldTime() >= GetFireTime())
        {
            return Duration;
        }

        return _timeElapsedBeforeCancel ??
               _timeElapsedBeforePause ??
               GetWorldTime() - _startTime;
    }

    public float GetTimeRemaining()
    {
        return Duration - GetTimeElapsed();
    }

    public float GetRatioComplete()
    {
        return GetTimeElapsed() / Duration;
    }

 
    public float GetRatioRemaining()
    {
        return GetTimeRemaining() / Duration;
    }

    #endregion

    #region Private Static Properties/Fields

    // responsible for updating all registered timers
    private static TimerManager _manager;

    #endregion

    #region Private Properties/Fields

    private bool IsOwnerDestroyed
    {
        get { return _hasAutoDestroyOwner && _autoDestroyOwner == null; }
    }

    private readonly Action _onComplete;
    private readonly Action<float> _onUpdate;
    private float _startTime;
    private float _lastUpdateTime;
    private float? _timeElapsedBeforeCancel;
    private float? _timeElapsedBeforePause;
    private readonly MonoBehaviour _autoDestroyOwner;
    private readonly bool _hasAutoDestroyOwner;

    #endregion

    #region Private Constructor (use static Register method to create new timer)

    public Timer(float duration, Action onComplete, Action<float> onUpdate,
        bool isLooped, bool usesRealTime, MonoBehaviour autoDestroyOwner)
    {
        Duration = duration;
        _onComplete = onComplete;
        _onUpdate = onUpdate;

        IsLooped = isLooped;
        UsesRealTime = usesRealTime;

        _autoDestroyOwner = autoDestroyOwner;
        _hasAutoDestroyOwner = autoDestroyOwner != null;

        _startTime = GetWorldTime();
        _lastUpdateTime = _startTime;
    }

    #endregion

    #region Private Methods

    private float GetWorldTime()
    {
        return UsesRealTime ? Time.realtimeSinceStartup : Time.time;
    }

    private float GetFireTime()
    {
        return _startTime + Duration;
    }

    private float GetTimeDelta()
    {
        return GetWorldTime() - _lastUpdateTime;
    }

    private void Update()
    {
        if (IsDone)
        {
            return;
        }

        if (IsPaused)
        {
            _startTime += GetTimeDelta();
            _lastUpdateTime = GetWorldTime();
            return;
        }

        _lastUpdateTime = GetWorldTime();

        if (_onUpdate != null)
        {
            _onUpdate(GetTimeElapsed());
        }

        if (GetWorldTime() >= GetFireTime())
        {

            if (_onComplete != null)
            {
                _onComplete();
            }

            if (IsLooped)
            {
                _startTime = GetWorldTime();
            }
            else
            {
                IsCompleted = true;
            }
        }
    }

    #endregion

    #region Manager Class (implementation detail, spawned automatically and updates all registered timers)

    private class TimerManager : MonoBehaviour
    {
        private List<Timer> _timers = new List<Timer>();

        // buffer adding timers so we don't edit a collection during iteration
        private List<Timer> _timersToAdd = new List<Timer>();

        public void RegisterTimer(Timer timer)
        {
            _timersToAdd.Add(timer);
        }

        public void CancelAllTimers()
        {
            foreach (Timer timer in _timers)
            {
                timer.Cancel();
            }

            _timers = new List<Timer>();
            _timersToAdd = new List<Timer>();
        }

        // update all the registered timers on every frame
        private void Update()
        {
            UpdateAllTimers();
        }

        private void UpdateAllTimers()
        {
            if (_timersToAdd.Count > 0)
            {
                _timers.AddRange(_timersToAdd);
                _timersToAdd.Clear();
            }

            foreach (Timer timer in _timers)
            {
                timer.Update();
            }

            _timers.RemoveAll(t => t.IsDone);
        }
    }

    #endregion

}
}


