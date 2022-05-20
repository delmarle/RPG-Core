using System;
using System.Collections;
using System.Threading;

namespace Station
{
    public class MultiThreadedProxy : ITaskProxy
    {
        public void Execute(Func<IEnumerator> executeFunc)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.CurrentThread.IsBackground = true;
                Fiber fiber = new Fiber(executeFunc.Invoke());
                while (fiber.Step())
                {
                }
            });
        }
    }
}