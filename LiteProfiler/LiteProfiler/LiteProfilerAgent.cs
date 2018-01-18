using System;
using System.Diagnostics;


namespace LiteProfile
{
    public class LiteProfilerAgent : IDisposable
    {
        public LiteProfilerAgent()
        {
            LiteProfiler.StartTiming();
        }

        [Conditional("DEBUG")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                LiteProfiler.StopTiming();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}



