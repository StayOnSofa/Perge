using System;
using System.Threading;

namespace Core.Generals
{
    public class Threading
    {
        private int _maxThreads = 0;
        private bool _inWork;

        private bool[] _workers;
        
        public Threading(int maxThreads)
        {
            _inWork = true;
            _maxThreads = maxThreads;

            _workers = new bool[maxThreads];
        }

        public void StartThreads(int index, Action threadingWork)
        {
            if (index >= _maxThreads)
                throw new Exception($"Threads more > {_maxThreads} max");
            
            _workers[index] = true;
            _inWork = true;

            ThreadPool.QueueUserWorkItem((c) =>
            {
                threadingWork?.Invoke();
                _workers[index] = false;
                CheckFinish();
            });
        }

        private void CheckFinish()
        {
            bool isFinish = true;
            
            for (int i = 0; i < _workers.Length; i++)
            {
                if (_workers[i] == true)
                {
                    isFinish = false;
                    break;
                }
            }

            if (isFinish)
            {
                _inWork = false;
            }
        }

        public bool InWork()
        {
            return _inWork;
        }
    }
}