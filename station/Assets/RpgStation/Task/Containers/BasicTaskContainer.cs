using Station;
using System;
using System.Collections.Generic;

namespace Station
{
    public class BasicTaskContainer : BasicTask<Dictionary<ITask, object>>, ITaskContainer
    {
        public List<ITask> Tasks;

        public BasicTaskContainer()
        {
            StepTaskEndCallback = new TaskEndEventInternal();
            StepTaskEndCallbackInternal = new TaskEndEventInternal();
            StepTaskPreStartCallback = new TaskPreStartStepEvent<Dictionary<ITask, object>>();
        }

        protected TaskPreStartStepEvent<Dictionary<ITask, object>> StepTaskPreStartCallback { get; private set; }
        protected TaskEndEventInternal StepTaskEndCallback { get; private set; }
        protected TaskEndEventInternal StepTaskEndCallbackInternal { get; private set; }

        public void AddTask(ITask task)
        {
            if (Tasks == null)
            {
                Tasks = new List<ITask>();
            }

            Tasks.Add(task);
        }

        public void RemoveTask(ITask task)
        {
            if (Tasks != null)
                Tasks.Remove(task);
        }

        public ITask SetStepPreStartCallback(
            StationAction<ITemplateTask<Dictionary<ITask, object>>, ITask> stepPreStartCallback)
        {
            StepTaskPreStartCallback.AddListener(stepPreStartCallback);
            return this;
        }

        public ITask SetStepCallback(StationAction<ITask, object, Exception, object> stepCallback)
        {
            StepTaskEndCallback.AddListener(stepCallback);
            return this;
        }

        public ITask SetStepCallbackInternal(StationAction<ITask, object, Exception, object> stepCallbackInternal)
        {
            StepTaskEndCallbackInternal.AddListener(stepCallbackInternal);
            return this;
        }
    }
}