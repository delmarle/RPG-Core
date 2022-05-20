using System;
using System.Collections;
using System.Collections.Generic;

namespace Station
{
    public class SerialTask : BasicTaskContainer
    {
        private bool _currentTaskFinished = true;
        private int _currentTaskIndex;
        private int _tasksCount;
        private List<ITask>.Enumerator _tasksEnumerator;

        protected override IEnumerator HandleExecute()
        {
            if (Result == null)
                Result = new Dictionary<ITask, object>();

            if (Tasks != null && Tasks.Count > 0)
            {
                _tasksEnumerator = Tasks.GetEnumerator();
                _tasksCount = Tasks.Count;
                _currentTaskIndex = 0;
                yield return CheckChildTasks();
            }

            if (Status != TaskStatus.Canceled)
                FinishTask(Result);
        }

        private IEnumerator CheckChildTasks()
        {
            while ((Status == TaskStatus.Running || Status == TaskStatus.Resumed) && _currentTaskFinished &&
                   _tasksEnumerator.MoveNext())
            {
                ExecuteEachChildtask();
                while (!_currentTaskFinished)
                {
                    yield return null;
                }
            }

            while (_currentTaskIndex < Tasks.Count && !_currentTaskFinished)
            {
                yield return null;
            }
        }


        private void ExecuteEachChildtask()
        {
            _currentTaskIndex++;
            if (_tasksEnumerator.Current != null)
            {
                StepTaskPreStartCallback.Invoke(this, _tasksEnumerator.Current);
                _tasksEnumerator.Current.SetEndCallbackInternal(OnChildTaskFinished)
                    .SetProgressCallback(OnChildTaskProgress);
                if (_tasksEnumerator.Current is ITaskContainer)
                    (_tasksEnumerator.Current as ITaskContainer).SetStepCallbackInternal(OnChildStepTaskFinished);
                _currentTaskFinished = false;
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
            SendBackProgress(progress);
        }

        private void SendBackProgress(float currentTaskProgress)
        {
            float currentProgress = ((_currentTaskIndex - 1) + currentTaskProgress) / _tasksCount;
            SendProgress(currentProgress);
        }

        private void SetChildTaskResult(ITask task, object result, Exception error, object clientParams)
        {
            if (_tasksEnumerator.Current != null)
            {
                Result.Add(_tasksEnumerator.Current, result);
                if (StepTaskEndCallbackInternal != null)
                    StepTaskEndCallbackInternal.Invoke(task, result, error, clientParams);
                if (StepTaskEndCallback != null)
                    StepTaskEndCallback.Invoke(task, result, error, clientParams);
            }
        }

        private void OnChildTaskFinished(ITask task, object result, Exception error, object clientParams)
        {
            SetChildTaskResult(task, result, error, clientParams);
            CheckChildTaskResult(task, result, error, clientParams);
            _currentTaskFinished = true;
        }

        protected virtual void CheckChildTaskResult(ITask task, object result, Exception error, object clientParams)
        {
            if (_tasksEnumerator.Current != null && _tasksEnumerator.Current.Status == TaskStatus.Error &&
                error != null && _tasksEnumerator.Current.IsCritical)
            {
                FinishTaskWithError("Critical Task Failed", error);
            }
        }

        protected override void OnPaused()
        {
            base.OnPaused();
            if (_tasksEnumerator.Current != null)
                _tasksEnumerator.Current.Pause();
        }

        protected override void OnResumed()
        {
            base.OnResumed();
            if (_tasksEnumerator.Current != null)
                _tasksEnumerator.Current.Resume();
        }

        protected override void OnCanceled()
        {
            base.OnCanceled();
            if (_tasksEnumerator.Current != null)
            {
                _tasksEnumerator.Current.Cancel();
            }
        }
    }
}