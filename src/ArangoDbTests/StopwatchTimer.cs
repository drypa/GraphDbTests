using System;
using System.Diagnostics;

namespace ArangoDbTests
{
    public class StopwatchTimer : IDisposable
    {
        private readonly Action<int> _callback;
        private readonly Stopwatch _stopwatch;
        public StopwatchTimer(Action<int> callback)
        {
            _callback = callback;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _callback((int)_stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}