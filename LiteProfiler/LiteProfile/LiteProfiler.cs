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


        private static readonly string NotSuspect = string.Empty;

        private static Dictionary<string, LiteProfilerLog> Ledger = new Dictionary<string, LiteProfilerLog>();
        private static Stopwatch Stopper;


        /// <summary>
        /// Start timer now (important for percentage calculation)
        /// </summary>
        [Conditional("DEBUG")]
        public static void Reset()
        {
            Stopper = Stopwatch.StartNew();
            Ledger.Clear();
        }


        private static bool TryGetProfileAddress(out string address)
        {
            StackTrace trace = new StackTrace(true);
            address = NotSuspect;

            // normally following loop should have been used: 
            // for (int i = 0; i <= trace.FrameCount; ++i)
            // but loop is changed for performance reasons AND for accurate calculations
            // 0, 1: current LiteProfiler class level - irrelevant
            // 2, 3: LiteProfilerAgent level or a real class if an agent is not used
            // 4: only required when called by StopTiming through an agent
            // 5, ...: too deep, meaning class is not under investigation
            for (int i = 2; i <= 4; ++i)
            {
                StackFrame frame = trace.GetFrame(i);
                MethodBase method = frame.GetMethod();
                string className = method.ReflectedType.Name;
                if (SuspiciousClasses.Contains(method.ReflectedType.Name))
                {
                    address = $"{method.ReflectedType.FullName}::{method.Name}";
                    return true;
                }

                // making sure to stop measuring wrong higher level classes
                if (className != "LiteProfilerAgent")
                {
                    return false;
                }
            }

            return false;
        }


        /// <summary>
        /// Start timing an specific part of code: Normally called by an agent measuring a whole function
        /// </summary>
        [Conditional("DEBUG")]
        public static void StartTiming()
        {
            string profileAddress;
            if (TryGetProfileAddress(out profileAddress))
            {
                if (!Ledger.ContainsKey(profileAddress))
                {
                    Ledger.Add(profileAddress, new LiteProfilerLog());
                }

                Ledger[profileAddress].StopperStartTimes.Push(Stopper.ElapsedMilliseconds);
            }
        }


        /// <summary>
        /// Stop timing code: Normally called by an agent measuring a whole function.
        /// </summary>
        [Conditional("DEBUG")]
        public static void StopTiming()
        {
            long stopTime = Stopper.ElapsedMilliseconds;
            string profileAddress;
            if (TryGetProfileAddress(out profileAddress))
            {
                if (Ledger.Count > 0)
                {
                    long startTime = Ledger[profileAddress].StopperStartTimes.Pop();
                    Ledger[profileAddress].MeasuredTimesSpent.Add(stopTime - startTime);
                }
            }
        }


        /// <summary>
        /// Create a "chackpoint" in code: designed to fit in a log file
        /// </summary>
        /// <param name="comment">Additional specific info to be recorded</param>
        /// <returns>A quote showing exaxt time and age since beginning of measurement</returns>
        public static LiteProfilerQuote GetQuote(string comment)
        {
            string profileAddress;
            if (!TryGetProfileAddress(out profileAddress))
            {
                return new LiteProfilerQuote()
                {
                    Comment = comment
                };
            }

            return new LiteProfilerQuote()
            {
                Address = profileAddress,
                Age = (Stopper != null) ? Stopper.ElapsedMilliseconds : 0,
                Comment = comment
            };
        }


        /// <summary>
        /// Create a report of final summry of measurements
        /// </summary>
        /// <returns>Full report of final summry of measurements</returns>
        public static LiteProfilerReport GetReport()
        {
            LiteProfilerReport report = new LiteProfilerReport()
            {
                TotalMeasured = (Stopper != null) ? Stopper.ElapsedMilliseconds : 0,
                Rows = new List<LiteProfilerReportRow>()
            };

            foreach (KeyValuePair<string, LiteProfilerLog> entry in Ledger)
            {
                report.Rows.Add(new LiteProfilerReportRow()
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
