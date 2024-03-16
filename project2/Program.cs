// See https://aka.ms/new-console-template for more information


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
            try
            {
                if (args.Length < 2 || args.Length > 3)
                {
                    Console.WriteLine(error_message);
                    return;
                }
                if (!validBits(args[0]))
                {
                    Console.WriteLine(error_message);
                    return;
                }
                if (!validOption(args[1]))
                {
                    Console.WriteLine(error_message);
                }
                if (args.Length == 3)
                {
                    if (!validCount(args[2]))
                    {
                        Console.WriteLine(error_message);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(error_message);
            }
        }

        static bool validBits(string bits)
        {
            try
            {
                int bitsNumber = int.Parse(bits);
                return bitsNumber >= 32 && bitsNumber % 8 == 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(error_message);
            }
            return true;
        }

        static bool validOption(string option)
        {
            return option == "prime" || option == "odd";
        }

        static bool validCount(string option)
        {
            return int.TryParse(option, out int result);
        }
    }
}
