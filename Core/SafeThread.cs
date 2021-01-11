using System;
using System.Threading;

namespace Core
{
    public class SafeThread
    {
        private readonly Thread _thread;

        private Action<Exception>? _exceptionHandler;
        public SafeThread(ThreadStart start)
        {
            _thread = new Thread(() => RunThreadStart(start));
        }

        private void RunThreadStart(ThreadStart start)
        {
            try
            {
                start.Invoke();
            }
            catch (Exception e)
            {
                _exceptionHandler?.Invoke(e);
            }
        }

        public SafeThread WithOnException(Action<Exception> handler)
        {
            _exceptionHandler = handler;
            return this;
        }

        public SafeThread Start()
        {
            _thread.Start();
            return this;
        }
        public void Join()
        {
            _thread.Join();
        }
    }
}