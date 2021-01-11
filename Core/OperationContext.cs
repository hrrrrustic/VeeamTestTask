using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Core
{
    public class OperationContext
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        public bool IsTerminated {get; private set; }
        public Exception? FirstException { get; private set; }
        public void Terminate(Exception ex)
        {
            if (IsTerminated)
                return;

            FirstException = ex;
            IsTerminated = true;
            _cts.Cancel();
        }

        public void ThrowIfCancellationRequested()
        {
            if (IsTerminated)
                throw new OperationCanceledException();
        }
        public CancellationToken GetCancellationToken() => _cts.Token;
        public void RethrowExceptionIfTerminated()
        {
            if(FirstException is not null)
                ExceptionDispatchInfo.Capture(FirstException).Throw();
        }
    }
}