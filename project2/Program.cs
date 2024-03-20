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
                var stopwatch = Stopwatch.StartNew();
                if (option == "prime")
                {
                    getPrimes(count, bits);
                }
                // get odds
                if (option == "odd")
                {
                    getOdds(count, bits);
                }
                Console.WriteLine($"Time to Generate: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fffffff}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(error_message);
            }
        }

        static BigInteger FindNumberOfPrimeFactors(BigInteger n)
        {
            List<BigInteger> factors = PrimeFactorization(n);

            // if the list is empty then n is prime
            if (factors.Count == 0)
            {
                return 2;
            }

            // total factors of
            BigInteger totalFactorsCount = 1;
            // number of consecutive primes ( 2, 2, 2, 3,... etc)
            BigInteger count = 1;

            // we started the counts at 1 because we need to start our for loop at index 1
            BigInteger previousFactor = factors[0];

            // for each index
            // if it is same factor (ex:(2,2)) increment the count
            // otherwise (totalFactors = totalFactors x count+1) and we get the new count of new prime
            // get the count of the final factor and return the total count
            for (int i = 1; i < factors.Count; i++)
            {
                if (factors[i] == previousFactor)
                {
                    count++;
                }
                else
                {
                    totalFactorsCount *= (count + 1);
                    previousFactor = factors[i];
                    count = 1;
                }
            }
            totalFactorsCount *= (count + 1);
            return totalFactorsCount;
        }

        static List<BigInteger> PrimeFactorization(BigInteger n)
        /**
            this is just standard prime factorization, we start at 3 because we ensure that the number
            is odd before putting it into this method.
        */
        {
            List<BigInteger> factors = new List<BigInteger>();

            BigInteger bigNumber = n;
            if (bigNumber.isProbablyPrime() == "probably prime")
            {
                return factors;
            }
            for (BigInteger i = 3; i * i <= n; i += 2)
            {
                while (bigNumber % i == 0 && i.isProbablyPrime() == "probably prime")
                {
                    factors.Add(i);
                    bigNumber /= i;
                    if (bigNumber.isProbablyPrime() == "probably prime")
                    {
                        return factors;
                    }
                }
            }

            return factors;
        }

        static void getPrimes(int count, string bits)
        {
            Counter counter = new Counter();

            while (counter.primesFound != count)
            {
                Thread thread = new Thread(() =>
                {
                    var bi = new BigInteger(getNumber(bits));
                    bi = BigInteger.Abs(bi); // no negative primes
                    if (bi.isProbablyPrime() == "probably prime")
                    {
                        lock (tasklock)
                        {
                            if (counter.primesFound < count)
                            {
                                counter.updatePrimesFoundCount(1);
                                Console.WriteLine($"{counter.primesFound}: {bi}\n");
                            }
                        }
                    }
                });
                thread.Start();
            }
        }

        static void getOdds(int count, string bits)
        /**
            method takes care of odd factorization, takes in the count and bits and does the rest
        */
        {
            List<BigInteger> bigIntPrimeFactors = new List<BigInteger>();
            Counter counter = new Counter();

            Parallel.For(
                0,
                count,
                i =>
                {
                    // get non-negative odd number
                    var bi = new BigInteger(getNumber(bits));
                    bi = BigInteger.Abs(bi);

                    if (bi % 2 == 0)
                    {
                        bi -= 1;
                    }

                    BigInteger numberFactors = FindNumberOfPrimeFactors(bi);
                    lock (counter)
                    {
                        counter.updateOddsCompleted(1);
                        Console.WriteLine($"{counter.oddsCompleted}: {bi}");
                        Console.WriteLine($"Number of factors: {numberFactors}");
                    }
                }
            );
        }

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

        public static string isProbablyPrime(this BigInteger n, int k = 10)
        {
            if (n <= 3)
            {
                return "probably prime";
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
        public int oddsCompleted = 0;

        public void updatePrimesFoundCount(int increment)
        {
            primesFound += increment;
        }

        public void updateOddsCompleted(int increment)
        {
            oddsCompleted += increment;
        }
    }
}
