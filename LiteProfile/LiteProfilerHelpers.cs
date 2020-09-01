using System;
using System.Collections.Generic;
using System.Text;


namespace LiteProfile
{
    /// <summary>
    /// List of all calls for 1 specific method
    /// </summary>
    public class LiteProfilerLog
    {
        public Stack<long> StopperStartTimes = new Stack<long>();
        public List<long> MeasuredTimesSpent = new List<long>();
    }


    /// <summary>
    /// An arbitrary recording checkpoint
    /// </summary>
    public class LiteProfilerQuote
    {
        public string Address { get; set; }
        public long Age { get; set; }
        public string Comment { get; set; }


        public LiteProfilerQuote()
        {
            Comment = string.Empty;
        }


        public string PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{DateTime.Now.ToString("yyyy_dd_MM__HH:mm:ss")}:\t");
            builder.Append($"{this.Address}\t");
            builder.Append($"Age = {this.Age}\t");
            builder.Append($"{this.Comment}");
            return builder.ToString();
        }
    }


    /// <summary>
    /// Completed measurements for each specific method
    /// </summary>
    public struct LiteProfilerReportRecord
    {
        public string Address { get; set; }
        public int CallsCount { get; set; }
        public long TotalSpent { get; set; }
    }


    /// <summary>
    /// Full report of all investigated methods
    /// </summary>
    public class LiteProfilerReport
    {
        public long TotalMeasured { get; set; }
        public List<LiteProfilerReportRecord> Records { get; set; }

        public const int Column_Width_Address = -40;
        public const int Column_Width_Calls = 13;
        public const int Column_Width_Age_ms = 17;
        public const int Column_Width_Percent = 17;


        public string PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("================================================================================================");
            builder.AppendLine();

            builder.Append($"{"Function Name", Column_Width_Address}");
            builder.Append($"{"Times Called", Column_Width_Calls}");
            builder.Append($"{"Total CPU [ms]", Column_Width_Age_ms}");
            builder.Append($"{"Total CPU [%]", Column_Width_Percent}");
            builder.AppendLine();

            builder.Append($"{AppDomain.CurrentDomain.FriendlyName, Column_Width_Address}");
            builder.Append($"{1, Column_Width_Calls}");
            builder.Append($"{this.TotalMeasured, Column_Width_Age_ms}");
            builder.Append($"{"100.00 %", Column_Width_Percent}"); 
            builder.AppendLine();

            foreach (LiteProfilerReportRecord record in this.Records)
            {
                builder.Append($"{record.Address, Column_Width_Address}");
                builder.Append($"{record.CallsCount, Column_Width_Calls}");
                builder.Append($"{record.TotalSpent, Column_Width_Age_ms}");
                builder.Append($"{Decimal.Divide(record.TotalSpent, this.TotalMeasured) * 100, Column_Width_Percent - 2:N2}{" %"}");
                builder.AppendLine();
            }

            builder.Append("================================================================================================");
            builder.AppendLine();

            return builder.ToString();
        }
    }
}
