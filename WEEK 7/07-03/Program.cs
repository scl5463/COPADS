using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;


namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var history1 = new StockHistory
            {
                price = 16.44,
                when = DateTime.Today
            };
            var history2 = new StockHistory
            {
                price = 13.44,
                when = DateTime.Today.AddDays(-5)
            };

            var s = new Stock
            {
                name = "RIT",
                price = 37.80,
                history = new List<StockHistory>
                {
                    history1,
                    history2
                }
            };

            var prog = new Program();
            prog.startServer(s);
            Console.ReadKey();


        }

        internal async void startServer(Stock s)
        {
            var listener = TcpListener.Create(3003);
            listener.Start();
            for(;;) {
                var client = await listener.AcceptTcpClientAsync();

                string json = JsonConvert.SerializeObject(s, Formatting.Indented);
                var sr = new StreamWriter(client.GetStream());
                sr.Write(json);
                sr.Flush();
                sr.Close();
            }
        }
    }

 

}
