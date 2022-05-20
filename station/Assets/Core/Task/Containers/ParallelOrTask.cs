using System;
using System.Collections;
using System.Collections.Generic;

namespace Station
{
    public class ParallelOrTask : BasicTaskContainer
    {
        private int _tasksCount;
        private List<ITask>.Enumerator _tasksEnumerator;
        private int _tasksFinished;
        private Dictionary<ITask, float> _tasksProgressCallbacks;

        protected override IEnumerator HandleExecute()
        {
            if (Result == null)
                Result = new Dictionary<ITask, object>();
            if (Tasks != null && Tasks.Count > 0)
            {
                _tasksEnumerator = Tasks.GetEnumerator();
                _tasksCount = Tasks.Count;
                _tasksFinished = 0;
                _tasksProgressCallbacks = new Dictionary<ITask, float>();
                IEnumerator checkChildTasks = CheckChildTasks();
                while (checkChildTasks.MoveNext())
                {
                    yield return null;
                }
            }
            else
            {
                FinishTask(Result);
            }
        }

        private IEnumerator CheckChildTasks()
        {
            while (Status == TaskStatus.Running || Status == TaskStatus.Resumed)
            {
                while (_tasksEnumerator.MoveNext())
                {
                    ExecuteEachChildtask();
                }

                while (_tasksFinished != 1)
                {
                    yield return null;
                }

                FinishTask(Result);
            }
        }

        private void ExecuteEachChildtask()
        {
            if (_tasksEnumerator.Current != null)
            {
                StepTaskPreStartCallback.Invoke(this, _tasksEnumerator.Current);
                _tasksEnumerator.Current
                    .SetEndCallbackInternal(OnChildTaskFinished(_tasksEnumerator.Current))
                    .SetProgressCallback(OnChildTaskProgress);
                if (_tasksEnumerator.Current != null && _tasksEnumerator.Current is ITaskContainer)
                    (_tasksEnumerator.Current as ITaskContainer).SetStepCallbackInternal(OnChildStepTaskFinished);
                _tasksEnumerator.Current.Execute();
            }
        }

        private void OnChildStepTaskFinished(ITask task, object result, Exception error, object clientParams)
        {
            if (StepTaskEndCallbackInternal != null)
                StepTaskEndCallbackInternal.Invoke(task, result, error, clientParams);
            if (StepTaskEndCallback != null)
                StepTaskEndCallback.Invoke(task, result, error, clientParams);
        }

        private void OnChildTaskProgress(ITask task, float progress)
        {
            SendProgress(progress);
        }

        private StationAction<ITask, object, Exception, object> OnChildTaskFinished(ITask childTask)
        {
            return (task, result, error, clientParams) =>
            {
                Result.Add(childTask, result);
                if (StepTaskEndCallbackInternal != null)
                    StepTaskEndCallbackInternal.Invoke(task, result, error, clientParams);
                if (StepTaskEndCallback != null)
                    StepTaskEndCallback.Invoke(task, result, error, clientParams);
                _tasksFinished++;
                if (error != null && childTask.IsCritical)
                {
                    FinishTaskWithError("Critical Task Failed", error);
                }
            };
        }

        protected override void OnPaused()
        {
            base.OnPaused();
            List<ITask>.Enumerator tasks = Tasks.GetEnumerator();
            while (tasks.MoveNext())
            {
                if (tasks.Current != null)
                    tasks.Current.Pause();
            }
        }

        protected override void OnResumed()
        {
            base.OnResumed();
            List<ITask>.Enumerator tasks = Tasks.GetEnumerator();
            while (tasks.MoveNext())
            {
                if (tasks.Current != null)
                    tasks.Current.Pause();
            }
        }

        protected override void OnCanceled()
        {
            List<ITask>.Enumerator tasks = Tasks.GetEnumerator();
            while (tasks.MoveNext())
            {
                if (tasks.Current != null)
                    tasks.Current.Cancel();
            }

            base.OnCanceled();
        }
    }
}