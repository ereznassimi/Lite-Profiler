using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;


namespace LiteProfile
{
    public static class LiteProfiler
    {
        /// <summary>
        /// Set of candidate classes to be investigated
        /// </summary>
        public static HashSet<string> SuspiciousClasses { get; set; }

        /// <summary>
        /// This flag should be used to measure specific classes only
        /// </summary>
        public static bool ShouldFilterClasses;


        private static Dictionary<string, LiteProfilerLog> Diary = new Dictionary<string, LiteProfilerLog>();
        private static Stopwatch Stopper;

        private const int InjectedDepthByTimer = 2;
        internal const int InjectedDepthByAgent = InjectedDepthByTimer + 1;

        /// <summary>
        /// Start timer now (important for percentage calculation)
        /// </summary>
        [Conditional("DEBUG")]
        public static void Reset(bool filterClasses = false)
        {
            LiteProfiler.Stopper = Stopwatch.StartNew();
            LiteProfiler.Diary.Clear();
            LiteProfiler.ShouldFilterClasses = filterClasses;
        }


        private static string GetProfileAddress(int frameIndex)
        {
            StackTrace trace = new StackTrace(fNeedFileInfo: true);
            StackFrame frame = trace.GetFrame(frameIndex);
            MethodBase method = frame.GetMethod();
            string className = method.ReflectedType.Name;

            if (LiteProfiler.ShouldFilterClasses &&
                !LiteProfiler.SuspiciousClasses.Contains(method.ReflectedType.Name))
            {
                return string.Empty;
            }

            return $"{method.ReflectedType.FullName}::{method.Name}";
        }


        /// <summary>
        /// Start timing an specific part of code: Normally called by an agent measuring a whole function
        /// </summary>
        [Conditional("DEBUG")]
        internal static void StartTimer(int frameIndex = InjectedDepthByTimer)
        {
            string profileAddress = LiteProfiler.GetProfileAddress(frameIndex);
            if (!string.IsNullOrWhiteSpace(profileAddress))
            {
                if (!LiteProfiler.Diary.ContainsKey(profileAddress))
                {
                    LiteProfiler.Diary.Add(profileAddress, new LiteProfilerLog());
                }

                LiteProfiler.Diary[profileAddress].StopperStartTimes.Push(Stopper.ElapsedMilliseconds);
            }
        }


        /// <summary>
        /// Stop timing code: Normally called by an agent measuring a whole function.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void StopTimer(int frameIndex = InjectedDepthByTimer)
        {
            long stopTime = LiteProfiler.Stopper.ElapsedMilliseconds;
            string profileAddress = GetProfileAddress(frameIndex);
            if (!string.IsNullOrWhiteSpace(profileAddress))
            {
                if (LiteProfiler.Diary.Count > 0)
                {
                    long startTime = LiteProfiler.Diary[profileAddress].StopperStartTimes.Pop();
                    LiteProfiler.Diary[profileAddress].MeasuredTimesSpent.Add(stopTime - startTime);
                }
            }
        }


        /// <summary>
        /// Create a "checkpoint" in code: designed to fit in a log file
        /// </summary>
        /// <param name="comment">Additional specific info to be recorded</param>
        /// <returns>A quote showing exact time and age since beginning of measurement</returns>
        public static LiteProfilerQuote GetQuote(string comment)
        {
            string profileAddress = LiteProfiler.GetProfileAddress(InjectedDepthByTimer);
            if (!string.IsNullOrWhiteSpace(profileAddress))
            {
                return new LiteProfilerQuote()
                {
                    Comment = comment
                };
            }

            return new LiteProfilerQuote()
            {
                Address = profileAddress,
                Age = (Stopper != null) ? LiteProfiler.Stopper.ElapsedMilliseconds : 0,
                Comment = comment
            };
        }


        /// <summary>
        /// Create a report of final summary of measurements
        /// </summary>
        /// <returns>Full report of final summary of measurements</returns>
        public static LiteProfilerReport GetReport()
        {
            LiteProfilerReport report = new LiteProfilerReport()
            {
                TotalMeasured = (Stopper != null) ? Stopper.ElapsedMilliseconds : 0,
                Records = new List<LiteProfilerReportRecord>()
            };

            foreach (KeyValuePair<string, LiteProfilerLog> entry in Diary)
            {
                report.Records.Add(new LiteProfilerReportRecord()
                {
                    Address = entry.Key,
                    CallsCount = entry.Value.MeasuredTimesSpent.Count,
                    TotalSpent = entry.Value.MeasuredTimesSpent.Sum()
                });
            }

            return report;
        }
    }
}
