using System.Net.Http;

namespace project3
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            try
            {
                if (BrokerAndValidator.valid_argument(args))
                {
                    await BrokerAndValidator.process_argument(args, client);
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
        static async Task getKey(string email, HttpClient client)
        {
            Console.WriteLine($"{email}");
            try
            {
                HttpResponseMessage response = await client.GetAsync(
                    $"http://voyager.cs.rit.edu:5050/Key/{email}"
                );
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseBody))
                    {
                        Console.WriteLine("Empty response body");
                    }
                    else
                    {
                        Console.WriteLine(responseBody);
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch data. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public static async Task process_argument(string[] args, HttpClient client)
        {
            try
            {
                switch (args[0])
                {
                    case "keyGen":
                    case "sendKey":
                    case "sendMsg":
                    case "getMsg":
                        break;
                    case "getKey":
                        await getKey(args[1], client);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
