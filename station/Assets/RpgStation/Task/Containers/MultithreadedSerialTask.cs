using System;
using System.Collections;
using System.Collections.Generic;

namespace Station
{
    public class MultithreadedSerialTask : BasicTaskContainer
    {
        private ITask _currentTask;
        private int _currentTaskIndex;

        protected override IEnumerator HandleExecute()
        {
            Result = new Dictionary<ITask, object>();
            if (Tasks != null && Tasks.Count > 0)
            {
                _currentTaskIndex = -1;
                CheckChildTasks();
            }
            else
            {
                FinishTask(Result);
            }

            yield break;
        }

        private void CheckChildTasks()
        {
            if ((Status == TaskStatus.Running || Status == TaskStatus.Resumed) && _currentTaskIndex < Tasks.Count - 1)
            {
                _currentTaskIndex++;
                _currentTask = Tasks[_currentTaskIndex];
                ExecuteEachChildtask();
            }
            else if (Status != TaskStatus.Canceled)
            {
                FinishTask(Result);
            }
        }

        private void ExecuteEachChildtask()
        {
            if (Tasks != null && Tasks.Count > 0)
            {
                StepTaskPreStartCallback.Invoke(this, _currentTask);
                _currentTask.SetEndCallbackInternal(OnChildTaskFinished).SetProgressCallback(OnChildTaskProgress);
                if (_currentTask is ITaskContainer)
                    (_currentTask as ITaskContainer).SetStepCallbackInternal(OnChildStepTaskFinished);
                _currentTask.Execute();
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
            float currentProgress = ((_currentTaskIndex - 1) + currentTaskProgress) / Tasks.Count;
            SendProgress(currentProgress);
        }

        private void SetChildTaskResult(ITask task, object result, Exception error, object clientParams)
        {
            if (_currentTask != null)
            {
                Result.Add(_currentTask, result);
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
            CheckChildTasks();
        }

        protected virtual void CheckChildTaskResult(ITask task, object result, Exception error, object clientParams)
        {
            if (_currentTask != null && _currentTask.Status == TaskStatus.Error && error != null &&
                _currentTask.IsCritical)
            {
                FinishTaskWithError("Critical Task Failed", error);
            }
        }

        protected override void OnPaused()
        {
            base.OnPaused();
            if (_currentTask != null)
                _currentTask.Pause();
        }

        protected override void OnResumed()
        {
            base.OnResumed();
            if (_currentTask != null)
                _currentTask.Resume();
        }

        protected override void OnCanceled()
        {
            base.OnCanceled();
            if (_currentTask != null)
            {
                _currentTask.Cancel();
            }
        }
    }
}