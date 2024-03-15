using System;
using System.Collections.Generic;


namespace ConsoleApplication
{
    public class Stock
    {
        public string name { get; set; }
        public double price { get; set; }
        public List<StockHistory> history { get; set; }
    }

    public class StockHistory
    {
        public DateTime when { get; set; }
        public double price { get; set; }
    }
}
