using System;
using System.Collections;
using System.Collections.Generic;
using Station;
using UnityEngine;

namespace Station
{

    public class DefaultProxy : ITaskProxy
    {
        public void Execute(Func<IEnumerator> executeFunc)
        {
            IEnumerator invoke = executeFunc.Invoke();
            while (invoke.MoveNext())
            {
            }
        }
    }
    public class ProxyWithRunner : ITaskProxy
    {
        public static TaskRunner TaskRunner;
        public void Execute(Func<IEnumerator> executeFunc)
        {
            if (ProxyWithRunner.TaskRunner == null)
                ProxyWithRunner.TaskRunner = Station.GameInstance.RootGameObject.AddComponent<TaskRunner>();

            TaskRunner.Fibers.Add(new Fiber(executeFunc.Invoke()));
        }
    }

    public class TaskRunner : MonoBehaviour
    {
        public List<Fiber> Fibers = new List<Fiber>();

        public void Update()
        {
            for (int idx = Fibers.Count - 1; idx >= 0; idx--)
            {
                Fiber fiber = Fibers[idx];
                if (!fiber.Step())
                    Fibers.Remove(fiber);
            }
        }
    }
}