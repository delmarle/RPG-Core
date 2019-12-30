using Station;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Station
{
    public interface ITask : IPausable, ICancelable
    {
        string Name { get; set; }
        ITaskProxy Proxy { get; set; }
        TaskStatus Status { get; set; }
        bool IsCritical { get; set; }
        void Execute(object clientParams = null);
        ITask SetEndCallbackInternal(StationAction<ITask, object, Exception, object> endCallbackInternal);
        ITask SetProgressCallback(StationAction<ITask, float> progressCallback);

        bool IsSuccess { get; }
        bool IsRunning { get; }
        bool IsCompleted { get; }
        bool HasError { get; }  
    }

}

    public interface ITemplateTask<T> : ITask
    {
        T GetResult();
        ITask SetPreStartCallback(StationAction<ITemplateTask<T>> preStartCallback);
        ITask SetEndCallback(StationAction<ITemplateTask<T>, T, Exception, object> endCallback);
    }

    public interface ITaskProxy
    {
        void Execute(Func<IEnumerator> executeFunc);
    }

    public interface ITaskContainer
    {
        void AddTask(ITask task);
        void RemoveTask(ITask task);

        ITask SetStepPreStartCallback(
            StationAction<ITemplateTask<Dictionary<ITask, object>>, ITask> stepPreStartCallback);

        ITask SetStepCallback(StationAction<ITask, object, Exception, object> stepCallback);
        ITask SetStepCallbackInternal(StationAction<ITask, object, Exception, object> stepCallbackInternal);
    }

    public interface IPausable
    {
        void Pause();
        void Resume();
    }

    public interface ICancelable
    {
        void Cancel();
    }

    public class TaskPreStartEvent<T> : StationEvent<ITemplateTask<T>>
    {
    }

    public class TaskPreStartStepEvent<T> : StationEvent<ITemplateTask<T>, ITask>
    {
    }

    public class TaskEndEvent<T> : StationEvent<ITemplateTask<T>, T, Exception, object>
    {
    }

    public class TaskEndEventInternal : StationEvent<ITask, object, Exception, object>
    {
    }

    public class TaskProgressEvent : StationEvent<ITask, float>
    {
    }

    public enum TaskStatus
    {
        Created = 0,
        Started = 1,
        Running = 2,
        Paused = 3,
        Resumed = 4,
        Canceled = 5,
        Error = 6,
        Finished = 7
    };
