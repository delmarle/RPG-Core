using System;
using System.Collections.Generic;
using System.Threading;

namespace Station
{

    public class ThreadQueueSystem : BaseSystem
    {
        protected Queue<Action> AsyncTaskActions;
        private object _lockObject = new object();
        private Thread _mainThread;
        public bool IsMainThread => Thread.CurrentThread == _mainThread;

        protected override void OnInit()
        {
            _mainThread = Thread.CurrentThread;
            lock (_lockObject)
            {
                AsyncTaskActions = new Queue<Action>();
            }
        }

        protected override void OnDispose()
        {
            
        }

        protected override void OnDataBaseReady()
        {
            
        }

        public void PushAction(Action action)
        {
            lock (_lockObject)
            {
                AsyncTaskActions.Enqueue(action);
            }
        }

        void Update()
        {
            CheckAsyncActions();
        }

        private void CheckAsyncActions()
        {
            lock (_lockObject)
            {
                while (AsyncTaskActions.Count > 0)
                {
                    AsyncTaskActions.Dequeue()();
                }
            }
        }
    }
}