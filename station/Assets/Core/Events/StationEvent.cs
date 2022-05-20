using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Station
{
    public delegate void StationAction();
    public delegate void StationAction<T>(T arg);
    public delegate void StationAction<T1, T2>(T1 arg1, T2 arg2);
    public delegate void StationAction<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate void StationAction<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    
    public class StationEvent : UnityEvent
    {
        private Dictionary<StationAction, UnityAction> _listenerMap = new Dictionary<StationAction, UnityAction>();

        public void AddListener(StationAction listener)
        {
            if (listener == null)
                return;

            if (_listenerMap.ContainsKey(listener))
            {
                Debug.LogError( "Listener "+listener+" already added for the event "+ ToString());
                return;
            }

            UnityAction action = () => listener();
            _listenerMap.Add(listener, action);

            base.AddListener(action);
        }

        public void RemoveListener(StationAction listener)
        {
            if (_listenerMap.ContainsKey(listener))
            {
                base.RemoveListener(_listenerMap[listener]);
                _listenerMap.Remove(listener);
            }
        }

        public new void RemoveAllListeners()
        {
            base.RemoveAllListeners();
            _listenerMap.Clear();
        }

        public System.Type EventHandlerType => typeof(StationAction);
    }

    public partial class StationEvent<T> : UnityEvent<T>
    {
        private Dictionary<StationAction<T>, UnityAction<T>> _listenerMap = new Dictionary<StationAction<T>, UnityAction<T>>();

        public void AddListener(StationAction<T> listener)
        {
            if (listener == null)
                return;

            if (_listenerMap.ContainsKey(listener))
            {
                Debug.LogError( "Listener "+listener+" already added for the event "+ ToString());
                return;
            }

            UnityAction<T> action = arg => listener(arg);
            _listenerMap.Add(listener, action);

            base.AddListener(action);
        }

        public void RemoveListener(StationAction<T> listener)
        {
            if (_listenerMap.ContainsKey(listener))
            {
                base.RemoveListener(_listenerMap[listener]);
                _listenerMap.Remove(listener);
            }
        }

        public new void RemoveAllListeners()
        {
            base.RemoveAllListeners();
            _listenerMap.Clear();
        }
        public System.Type EventHandlerType => typeof(StationAction<T>);
    }

    public partial class StationEvent<T1, T2> : UnityEvent<T1, T2>
    {
        private Dictionary<StationAction<T1, T2>, UnityAction<T1, T2>> _listenerMap = new Dictionary<StationAction<T1, T2>, UnityAction<T1, T2>>();

        public void AddListener(StationAction<T1, T2> listener)
        {
            if (listener == null)
                return;

            if (_listenerMap.ContainsKey(listener))
            {
                Debug.LogError( "Listener "+listener+" already added for the event "+ ToString());
                return;
            }

            UnityAction<T1, T2> action = (arg1, arg2) => listener(arg1, arg2);
            _listenerMap.Add(listener, action);

            base.AddListener(action);
        }

        public void RemoveListener(StationAction<T1, T2> listener)
        {
            if (_listenerMap.ContainsKey(listener))
            {
                base.RemoveListener(_listenerMap[listener]);
                _listenerMap.Remove(listener);
            }
        }

        public new void RemoveAllListeners()
        {
            base.RemoveAllListeners();
            _listenerMap.Clear();
        }
        public System.Type EventHandlerType => typeof(StationAction<T1, T2>);
    }

    public partial class StationEvent<T1, T2, T3> : UnityEvent<T1, T2, T3>
    {
        private Dictionary<StationAction<T1, T2, T3>, UnityAction<T1, T2, T3>> _listenerMap = new Dictionary<StationAction<T1, T2, T3>, UnityAction<T1, T2, T3>>();

        public void AddListener(StationAction<T1, T2, T3> listener)
        {
            if (listener == null)
                return;

            if (_listenerMap.ContainsKey(listener))
            {
                Debug.LogError( "Listener "+listener+" already added for the event "+ ToString());
                return;
            }

            UnityAction<T1, T2, T3> action = (arg1, arg2, arg3) => listener(arg1, arg2, arg3);
            _listenerMap.Add(listener, action);

            base.AddListener(action);
        }

        public void RemoveListener(StationAction<T1, T2, T3> listener)
        {
            if (_listenerMap.ContainsKey(listener))
            {
                base.RemoveListener(_listenerMap[listener]);
                _listenerMap.Remove(listener);
            }
        }

        public new void RemoveAllListeners()
        {
            base.RemoveAllListeners();
            _listenerMap.Clear();
        }
        public System.Type EventHandlerType => typeof(StationAction<T1, T2, T3>);
    }

    public partial class StationEvent<T1, T2, T3, T4> : UnityEvent<T1, T2, T3, T4>
    {
        private Dictionary<StationAction<T1, T2, T3, T4>, UnityAction<T1, T2, T3, T4>> _listenerMap = new Dictionary<StationAction<T1, T2, T3, T4>, UnityAction<T1, T2, T3, T4>>();

        public void AddListener(StationAction<T1, T2, T3, T4> listener)
        {
            if (listener == null)
                return;

            if (_listenerMap.ContainsKey(listener))
            {
                Debug.LogError( "Listener "+listener+" already added for the event "+ ToString());
                return;
            }

            UnityAction<T1, T2, T3, T4> action = (arg1, arg2, arg3, arg4) => listener(arg1, arg2, arg3, arg4);
            _listenerMap.Add(listener, action);

            base.AddListener(action);
        }

        public void RemoveListener(StationAction<T1, T2, T3, T4> listener)
        {
            if (_listenerMap.ContainsKey(listener))
            {
                base.RemoveListener(_listenerMap[listener]);
                _listenerMap.Remove(listener);
            }
        }

        public new void RemoveAllListeners()
        {
            base.RemoveAllListeners();
            _listenerMap.Clear();
        }
        public System.Type EventHandlerType => typeof(StationAction<T1, T2, T3, T4>);
    }
}