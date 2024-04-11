using System.Net.Http;

namespace project3
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            try
            {
                if (BrokerAndValidator.valid_argument(args))
                {
                    BrokerAndValidator.start_class(args, client);
                }
            }
            catch
            {
                Console.WriteLine("caught it");
            }
        }
    }

    internal class BrokerAndValidator
    {
        public static void start_class(string[] args, HttpClient client)
        {
            try
            {
                Console.WriteLine("starting class");
            }
            catch
            {
                Console.WriteLine("Error in the start class");
            }
        }

        public static bool valid_argument(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    return false;
                }

                switch (args[0])
                {
                    case "keyGen":
                        return int.TryParse(args[1], out _);
                    case "sendKey":
                    case "getKey":
                    case "sendMsg":
                    case "getMsg":
                        return args[1] is string;
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
