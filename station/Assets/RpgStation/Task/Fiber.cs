using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Station
{
    public class Fiber
    {
        private readonly Stack<IEnumerator> _stackFrame =
            new Stack<IEnumerator>();

        private IEnumerator _currentRoutine;

        private bool _isStepping;

        public Fiber(IEnumerator entryPoint)
        {
            this._currentRoutine = entryPoint;
        }

        public bool Step()
        {
            if (_isStepping) return true;
            _isStepping = true;
            if (_currentRoutine != null && _currentRoutine.MoveNext())
            {
                var subRoutine = _currentRoutine.Current
                    as IEnumerator;
                if (subRoutine != null)
                {
                    _stackFrame.Push(_currentRoutine);
                    _currentRoutine = subRoutine;
                }
            }
            else if (_stackFrame.Count > 0)
            {
                _currentRoutine = _stackFrame.Pop();
            }
            else
            {
                if (_currentRoutine != null)
                {
                    OnFiberTerminated(
                        new FiberTerminatedEventArgs(
                            _currentRoutine.Current
                        )
                    );
                }

                _isStepping = false;
                return false;
            }

            _isStepping = false;
            return true;
        }

        public object Current => _currentRoutine?.Current;

        public event EventHandler<FiberTerminatedEventArgs>
            FiberTerminated;

        private void OnFiberTerminated(
            FiberTerminatedEventArgs e)
        {
            var handler = FiberTerminated;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class FiberTerminatedEventArgs
        : EventArgs
    {
        private readonly object _result;

        public FiberTerminatedEventArgs(object result)
        {
            this._result = result;
        }

        public object Result
        {
            get { return this._result; }
        }
    }
}