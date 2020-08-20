using LiteProfile;
using System;
using System.Collections.Generic;
using System.IO;


namespace Demo
{
    class Program
    {
        private const long n = 1000;

        static void Main(string[] args)
        {
            LiteProfiler.Reset();
            LiteProfiler.SuspiciousClasses = new HashSet<string>(File.ReadAllLines((args.Length > 0) ? args[0] : "classes.txt"));

            Console.WriteLine($"n = {n}");

            using (LiteProfilerAgent agent = new LiteProfilerAgent())
            {
                Demo_1(n);
                Demo_2(n);
            }

            Console.Write(LiteProfiler.GetReport().PrettyPrint());
            NextDemo();

            Demo_3(n);
            Console.ReadKey();
        }

        public static void Demo_1(long n)
        {
            using (LiteProfilerAgent agent = new LiteProfilerAgent())
            {
                long prime = PrimeNumbers.Find(n);
                Console.SetCursorPosition(0, 2);
                Console.WriteLine($"Prime #{n} = {prime}");
            }

            Console.Write(LiteProfiler.GetReport().PrettyPrint());
            NextDemo();
        }

        public static void Demo_2(long n)
        {
            using (LiteProfilerAgent agent = new LiteProfilerAgent())
            {
                long heavy = TimeConsuming.HeavyCalculation(n);
                Console.WriteLine($"Heavy Calculation({n}) = {heavy}");
            }
        }

        public static void Demo_3(long n)
        {
            LiteProfiler.Reset(LiteProfiler.DoFilterClasses);
            using (LiteProfilerAgent agent = new LiteProfilerAgent())
            {
                long heavy = TimeConsuming.HeavyCalculation(n);
                Console.WriteLine($"Heavy Calculation({n}) = {heavy}");
            }

            Console.Write(LiteProfiler.GetReport().PrettyPrint());
        }

        private static void NextDemo()
        {
            Console.WriteLine();
            Console.WriteLine("Please press any key for next demo.");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
