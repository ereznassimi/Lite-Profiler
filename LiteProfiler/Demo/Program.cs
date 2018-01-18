using LiteProfile;
using System;
using System.Collections.Generic;


namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            LiteProfiler.Reset();
            LiteProfiler.SuspiciousClasses = new HashSet<string> { "PrimeNumbers", "TimeConsuming", "Program" };

            Random rnd = new Random();
            long n = rnd.Next(1, TimeConsuming.Max);
            Console.WriteLine($"n = {n}");

            using (LiteProfilerAgent agent = new LiteProfilerAgent())
            {
                long prime = PrimeNumbers.Find(n);
                Console.WriteLine($"Prime#({n}) = {prime}");

                long heavy = TimeConsuming.HeavyCalculation(n);
                Console.WriteLine($"Heavy Calculationn({n}) = {heavy}");
            }

            Console.WriteLine("=================================");
            Console.Write(LiteProfiler.GetReport().PrettyPrint());
            Console.WriteLine("=================================");

            Console.ReadLine();
        }
    }
}
