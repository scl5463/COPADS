using System.Numerics;
using System.Security.Cryptography;

namespace project2
{
    internal class Program
    {
        static string error_message =
            "\ndotnet run <bits> <option> <count>\n"
            + "     - bits - the number of bits of the number to be generated, must be a multiple of 8 and at least 32 bits.\n"
            + "     - option - 'odd' or 'prime' (the type of numbers to be generated)\n"
            + "     - count - the count of numbers to generate, defaults to 1.\n";

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
                string option = args[1];

                var bi = new BigInteger(getNumber(bits));
                Console.WriteLine(bi);
                // get primes

                // get odds
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
                Console.WriteLine(error_message);
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
        public static bool IsProbablyPrime(this BigInteger value, int k = 10)
        {
            // put prime logic here
            return false;
        }
    }
}
