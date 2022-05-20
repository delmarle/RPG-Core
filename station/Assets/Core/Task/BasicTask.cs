using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Station
{
    public class TaskDefault
    {
        public static Type DefaultProxy;
        public static bool ENABLE_TIME_TRACE;
    }

    public class BasicTask<T> : ITemplateTask<T>
    {
        private List<string> _allResultBindings;
        private long _startTime;
        private string _name;
        private bool _enableTraceTime;

        public BasicTask()
        {
            EndCallback = new TaskEndEvent<T>();
            EndCallbackInternal = new TaskEndEventInternal();
            ProgressCallback = new TaskProgressEvent();
            PreStartCallback = new TaskPreStartEvent<T>();
            Status = TaskStatus.Created;
        }

        public object ClientParams { get; set; }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return GetType().Name + "_" + typeof(T).Name;
                }

                return _name;
            }
            set { _name = value; }
        }
        public TaskStatus Status { get; set; }

        public bool IsRunning => Status == TaskStatus.Running;
        public bool IsSuccess => Status == TaskStatus.Finished;

        public bool IsCompleted => Status == TaskStatus.Finished || Status == TaskStatus.Error;
        public bool HasError => Status == TaskStatus.Error;


        private Exception Error { get; set; }
        private TaskPreStartEvent<T> PreStartCallback { get; set; }
        private TaskEndEvent<T> EndCallback { get; set; }
        private TaskEndEventInternal EndCallbackInternal { get; set; }
        private TaskProgressEvent ProgressCallback { get; set; }

        protected T Result { get; set; }

        public ITaskProxy Proxy { get; set; }
        public bool IsCritical { get; set; }
        public bool EnableLogs { get; set; }

        public bool EnableTimeTrace
        {
            get => _enableTraceTime || TaskDefault.ENABLE_TIME_TRACE;
            set => _enableTraceTime = value;
        }

        public ITask SetEndCallbackInternal(StationAction<ITask, object, Exception, object> endCallbackInternal)
        {
            
            EndCallbackInternal.AddListener(endCallbackInternal);
            return this;
        }

        public T GetResult()
        {
            return Result;
        }

        public ITask SetPreStartCallback(StationAction<ITemplateTask<T>> preStartCallback)
        {
            PreStartCallback.AddListener(preStartCallback);
            return this;
        }

        public Exception GetError()
        {
            return Error;
        }

        public ITask SetEndCallback(StationAction<ITemplateTask<T>, T, Exception, object> endCallback)
        {
            EndCallback.AddListener(endCallback);
            return this;
        }

        public ITask SetProgressCallback(StationAction<ITask, float> progressCallback)
        {
            ProgressCallback.AddListener(progressCallback);
            return this;
        }

        public void Execute(object clientParams = null)
        {
            if (clientParams != null)
                ClientParams = clientParams;
            UpdateExecuteStatus();
        }

        public void Pause()
        {
            Status = TaskStatus.Paused;
            UpdateExecuteStatus();
        }

        public void Resume()
        {
            Status = TaskStatus.Resumed;
            UpdateExecuteStatus();
        }

        public void Cancel()
        {
            if (Status == TaskStatus.Running)
            {
                Status = TaskStatus.Canceled;
                UpdateExecuteStatus();
            }
        }

        protected void PreExecute()
        {
            PreStartCallback.Invoke(this);
            Status = TaskStatus.Started;
            UpdateExecuteStatus();
        }

        protected void OnExecute()
        {
            StartExecution();
        }

        protected void StartExecution()
        {
            Status = TaskStatus.Running;
            UpdateExecuteStatus();
            if (Proxy == null)
            {
                if (TaskDefault.DefaultProxy != null)
                {
                    Proxy = (ITaskProxy) Activator.CreateInstance(TaskDefault.DefaultProxy);
                }
                else
                {
                    Proxy = new DefaultProxy();
                }
                
            }
            Proxy.Execute(HandleExecute);
        }

        protected virtual IEnumerator HandleExecute()
        {
            return null;
        }

        protected virtual void SendProgress(float progress)
        {
            if (ProgressCallback != null)
                ProgressCallback.Invoke(this, progress);
        }

        protected void OnRunning()
        {
        }

        protected void FinishTaskWithError(string errorMsg, Exception innerError = null)
        {
            if (Status == TaskStatus.Running)
            {
                Status = TaskStatus.Error;

                if (EnableLogs)
                {
                    if (IsCritical)
                    {
                        if (innerError == null)
                            ThrowError(errorMsg);
                        else
                            ThrowError(errorMsg, innerError.Message);
                    }
                    else
                    {
                        if (innerError == null)
                            ThrowWarning(errorMsg);
                        else
                            ThrowWarning(errorMsg, innerError.Message);
                    }
                }

                Error = new Exception(errorMsg, innerError);
                UpdateExecuteStatus();
            }
        }

        protected void FinishTaskWithError(Exception exception)
        {
            if (Status == TaskStatus.Running)
            {
                Status = TaskStatus.Error;
                if (IsCritical)
                {
                    if(exception==null)
                        ThrowError("");
                    else
                        ThrowError(exception.ToString());
                }
                else
                {
                    if (exception == null)
                        ThrowWarning("");
                    else
                        ThrowWarning(exception.ToString());
                }
                Error = exception;
                UpdateExecuteStatus();
            }
        }

        protected virtual void FinishTask(T result)
        {
            if (Status == TaskStatus.Running)
            {
                Result = result;
                Status = TaskStatus.Finished;
                UpdateExecuteStatus();
            }
        }

        protected void CancelTask()
        {
            if (Status == TaskStatus.Running)
            {
                Status = TaskStatus.Canceled;
                UpdateExecuteStatus();
            }
        }

        protected virtual void OnPaused()
        {
        }


        protected virtual void OnResumed()
        {
        }

        protected virtual void OnCanceled()
        {
            SendResponse();
        }

        protected void OnFinished()
        {
            if (EnableTimeTrace)
            {
                #if UNITY_EDITOR
                    UnityEngine.Debug.Log($"Task {Name} Complete Time : {DateTimeUtils.GetEpochTimeInMs() - _startTime} ms");
               #endif
            }
            SendResponse();
        }

        private void SendResponse()
        {
            if (EndCallbackInternal != null)
                EndCallbackInternal.Invoke(this, Result, Error, ClientParams);
            if (EndCallback != null)
                EndCallback.Invoke(this, Result, Error, ClientParams);
            if (ProgressCallback != null)
                ProgressCallback.Invoke(this, 1);
        }

        public void RemoveAllListeners()
        {
            EndCallback.RemoveAllListeners();
            EndCallbackInternal.RemoveAllListeners();
            ProgressCallback.RemoveAllListeners();
        }

        protected void UpdateExecuteStatus()
        {
            switch (Status)
            {
                case TaskStatus.Created:
                    PreExecute();
                    break;
                case TaskStatus.Started:
                    if (EnableTimeTrace)
                    {
                        _startTime = DateTimeUtils.GetEpochTimeInMs();
                    }
                    OnExecute();
                    break;
                case TaskStatus.Running:
                    OnRunning();
                    break;
                case TaskStatus.Paused:
                    OnPaused();
                    break;
                case TaskStatus.Resumed:
                    OnResumed();
                    break;
                case TaskStatus.Canceled:
                    OnCanceled();
                    break;
                case TaskStatus.Finished:
                    OnFinished();
                    break;
                case TaskStatus.Error:
                    OnFinished();
                    break;
            }
        }

        protected void ThrowError(string message)
        {
            Debug.LogError("Task failed name : "+ Name+" , with error "+message);
        }

        protected void ThrowError(string message,string subMessage)
        {
            Debug.LogError("Task failed name : "+ Name+" , with error "+message + " and sub message"+ subMessage);
        }

        protected void ThrowWarning(string message)
        {
          Debug.LogWarning(message);
        }

        protected void ThrowWarning(string message, string subMessage)
        {
            Debug.LogError("Task failed name : "+ Name+" , with error "+message + " and sub message"+ subMessage);
        }
    }
}