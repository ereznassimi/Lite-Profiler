using System;


namespace LiteProfile
{
    public class LiteProfilerAgent : IDisposable
    {
        public LiteProfilerAgent()
        {
            LiteProfiler.StartTimer(LiteProfiler.InjectedDepthByAgent);
        }

        public void Dispose()
        {
            LiteProfiler.StopTimer(LiteProfiler.InjectedDepthByAgent);
        }
    }
}
