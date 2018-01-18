using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;


namespace LiteProfile
{
    public class LiteProfilerItem
    {
        public Stack<long> StopperStartTimes = new Stack<long>();
        public List<long> MeasuredTimesSpent = new List<long>();
    }

    public static class LiteProfiler
    {
        public static HashSet<string> SuspiciousClasses { get; set; }

        private static readonly string NotSuspect = string.Empty;

        private static Dictionary<string, LiteProfilerItem> Ledger = new Dictionary<string, LiteProfilerItem>();
        private static Stopwatch Stopper;


        [Conditional("DEBUG")]
        public static void Reset()
        {
            Stopper = Stopwatch.StartNew();
            Ledger.Clear();
        }

        private static bool TryCreateProfileKey(out string key)
        {
            StackTrace trace = new StackTrace(true);
            key = NotSuspect;

            // starting from 2 and not 0 to skip this method and its caller
            for (int i = 2; i <= trace.FrameCount; ++i)
            {
                StackFrame frame = trace.GetFrame(i);
                MethodBase method = frame.GetMethod();
                string className = method.ReflectedType.Name;
                if (SuspiciousClasses.Contains(method.ReflectedType.Name))
                {
                    key = $"{method.ReflectedType.FullName}::{method.Name}";
                    return true;
                }

                if (className != "LiteProfilerAgent")
                {
                    return false;
                }
            }

            return false;
        }

        [Conditional("DEBUG")]
        public static void StartTiming()
        {
            string profileKey;
            if (TryCreateProfileKey(out profileKey))
            {
                if (!Ledger.ContainsKey(profileKey))
                {
                    Ledger.Add(profileKey, new LiteProfilerItem());
                }
                Ledger[profileKey].StopperStartTimes.Push(Stopper.ElapsedMilliseconds);
            }
        }

        [Conditional("DEBUG")]
        public static void StopTiming()
        {
            long stopTime = Stopper.ElapsedMilliseconds;
            string profileKey;
            if (TryCreateProfileKey(out profileKey))
            {
                if (Ledger.Count > 0)
                {
                    long startTime = Ledger[profileKey].StopperStartTimes.Pop();
                    Ledger[profileKey].MeasuredTimesSpent.Add(stopTime - startTime);
                }
            }
        }

        [Conditional("DEBUG")]
        public static void Quote()
        {
            string profileKey;
            if (TryCreateProfileKey(out profileKey))
            {
                Console.WriteLine($"Lite Profiler: {profileKey}: " + Stopper.ElapsedMilliseconds);
            }
        }

        [Conditional("DEBUG")]
        public static void Report()
        {
            foreach (KeyValuePair<string, LiteProfilerItem> entry in Ledger)
            {
                Console.WriteLine($"Lite Profiler: {entry.Key} * {entry.Value.MeasuredTimesSpent.Count} = {entry.Value.MeasuredTimesSpent.Sum()}");
            }
        }
    }
}
