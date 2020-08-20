using LiteProfile;
using System;


namespace Demo
{
    public static class TimeConsuming
    {
        public const int Max = 1000;

        public static Random random = new Random();

        public static long OneStep(long i)
        {
            using (LiteProfilerAgent agent = new LiteProfilerAgent())
            {
                long c = random.Next(1, Max);
                long result = ((i * c) % Max) + 1;
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"(({i} * {c}) % {Max}) + 1 = {result}");
                Console.WriteLine(LiteProfiler.GetQuote($"step starts with i = {i}").PrettyPrint());
                return result;
            }
        }

        public static long HeavyCalculation(long n)
        {
            using (LiteProfilerAgent agent = new LiteProfilerAgent())
            {
                long result = 1;
                for (long i = 0; i < n; ++i)
                {
                    result = OneStep(result);
                }

                return result;
            }
        }
    }
}
