using LiteProfile;
using System;


namespace Demo
{
    public static class PrimeNumbers
    {
        public static bool IsPrime(long i)
        {
            using (LiteProfilerAgent agent = new LiteProfilerAgent())
            {
                long b = 2;
                while (b * b <= i)
                {
                    if (i % b == 0)
                    {
                        return false;
                    }
                    b++;
                }

                return true;
            }
        }

        public static long Find(long n)
        {
            using (LiteProfilerAgent agent = new LiteProfilerAgent())
            {
                long count = 0;
                long a = 2;
                while (count < n)
                {
                    if (IsPrime(a))
                    {
                        count++;
                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine($"prime #{count} / {n} = {a}");
                    }
                    a++;
                }

                return (--a);
            }
        }
    }
}