using System;
using System.Collections.Generic;
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
                //when = new DateTime(2019, 7, 26)
                when = DateTime.Today
            };
            var history2 = new StockHistory
            {
                price = 13.44,
                //when = new DateTime(2019, 7, 21)
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

            string json = JsonConvert.SerializeObject(s, Formatting.Indented);
            Console.WriteLine(json);

            var dstock = JsonConvert.DeserializeObject<Stock>(json);
            Console.WriteLine(dstock.name);
            Console.WriteLine(dstock.price);
            
            foreach(var i in dstock.history)
            {
                Console.WriteLine(i.when);
                Console.WriteLine(i.price);
            }
            
                        
        }
    }

}
