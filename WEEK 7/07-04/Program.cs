using System;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;


namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var prog = new Program();
            prog.startClient();
            Console.ReadKey();
        }

        internal async void startClient()

        {
            var client = new TcpClient();
            await client.ConnectAsync("localhost", 3003);
            var sr = new StreamReader(client.GetStream());
            var rit = JsonConvert.DeserializeObject<Stock>(sr.ReadToEnd());
            
            Console.WriteLine(rit.name);
            Console.WriteLine(rit.price);

            foreach (var i in rit.history)
            {
                Console.WriteLine(i.when);
                Console.WriteLine(i.price);
            }
        }
    }

 

}
