using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace project2
{
    internal class Program
    {
        static string error_message =
            "\ndotnet run <bits> <option> <count>\n"
            + "     - bits - the number of bits of the number to be generated, must be a multiple of 8 and at least 32 bits.\n"
            + "     - option - 'odd' or 'prime' (the type of numbers to be generated)\n"
            + "     - count - the count of numbers to generate, defaults to 1.\n";
        static object tasklock = new object();

        static void Main(string[] args)
        {
            int count;
            try
            {
                if (
                    args.Length < 2
                    || args.Length > 3
                    || !validBits(args[0])
                    || !validOption(args[1])
                )
                {
                    Console.WriteLine(error_message);
                    return;
                }
                if (args.Length == 3)
                {
                    if (!validCount(args[2]))
                    {
                        Console.WriteLine(error_message);
                        return;
                    }
                    count = int.Parse(args[2]);
                }
                else
                {
                    count = 1;
                }
                string bits = args[0];
                Console.WriteLine($"BitLength:  {bits}  bits");
                string option = args[1];

                Counter counter = new Counter();
                // get primes
                if (option == "prime")
                {
                    var stopwatch = Stopwatch.StartNew();
                    while (counter.primesFound != count)
                    {
                        Thread thread = new Thread(() =>
                        {
                            var bi = new BigInteger(getNumber(bits));
                            bi = BigInteger.Abs(bi); // no negative primes
                            if (bi.IsProbablyPrime() == "probably prime")
                            {
                                lock (tasklock)
                                {
                                    if (counter.primesFound < count)
                                    {
                                        counter.UpdatePrimesFoundCount(1);
                                        Console.WriteLine($"{counter.primesFound}: {bi}\n");
                                    }
                                }
                            }
                        });
                        thread.Start();
                    }
                    Console.WriteLine(
                        $"Time to Generate: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fffffff}"
                    );
                }
                // get odds
                if (option == "odd") { }
            }
            catch
            {
                Console.WriteLine(error_message);
            }
        }

        static void getOdds(int number, int count) { }

        static bool validBits(string bits)
        {
            try
            {
                int bitsNumber = int.Parse(bits);
                return bitsNumber >= 32 && bitsNumber % 8 == 0;
            }
            catch
            {
                Console.WriteLine("bad bits, must be a number divisible by 8");
            }
            return true;
        }

        static bool validOption(string option)
        {
            return option == "prime" || option == "odd";
        }

        static Boolean validCount(string option)
        {
            return int.TryParse(option, out int result);
        }

        static byte[] getNumber(string bits)
        {
            int numberBytes = int.Parse(bits) / 8;
            byte[] randomBytes = new byte[numberBytes];
            RandomNumberGenerator generator = RandomNumberGenerator.Create();

            generator.GetBytes(randomBytes);

            return randomBytes;
        }
    }

    public static class BigIntegerExtensions
    {
        static BigInteger getRandomBigIntForPrimes(BigInteger max)
        /**
            gets random number between max and max-2
            @param max - the bigint max
            @return a - the bigint random number between max and max-2
        */
        {
            RandomNumberGenerator gen = RandomNumberGenerator.Create();
            int a = (max - 2).ToByteArray().Length;
            byte[] aSize = new byte[a];
            gen.GetBytes(aSize);
            BigInteger randomBigInt = new BigInteger(aSize);
            randomBigInt = BigInteger.Abs(randomBigInt);
            randomBigInt = (randomBigInt % (max - 2)) + 2;
            return randomBigInt;
        }

        public static string IsProbablyPrime(this BigInteger n, int k = 10)
        {
            if (n <= 3)
            {
                return "probbaly prime";
            }

            // d = n-1 / 2^(r)
            // mod until d%2 != 0 will give value of r and d
            BigInteger d = n - 1;
            BigInteger r = 0;
            while (d % 2 == 0)
            {
                d /= 2;
                r += 1;
            }

            int count = k;
            while (count > 0)
            {
                count--;
                BigInteger a = getRandomBigIntForPrimes(n);
                BigInteger x = BigInteger.ModPow(a, d, n);

                if (x == 1 || x == n - 1)
                {
                    continue;
                }
                if (!innerLoop(r, x, n))
                {
                    return "composite";
                }
            }
            return "probably prime";
        }

        public static bool innerLoop(BigInteger r, BigInteger x, BigInteger n)
        /**
        I wrote this function because the psuedo code sucks and theres no way to break out
        of a loop and continue that looks nice and i refuse to use goto since its bad practice
        */
        {
            while (r > 1)
            {
                r--;
                x = BigInteger.ModPow(x, 2, n);
                if (x == n - 1)
                {
                    return true;
                }
            }
            return false;
        }
    }

    internal class Counter
    {
        public int primesFound = 0;
        public int threadsSpawned = 0;

        public void UpdatePrimesFoundCount(int increment)
        {
            primesFound += increment;
        }

        public void addThread()
        {
            threadsSpawned += 1;
        }
    }
}
