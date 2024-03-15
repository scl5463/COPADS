using System;
using System.Collections.Generic;


namespace ConsoleApplication
{
    internal class Stock
    {
        public int? peoples {get; set;}
        public string name { get; set; }
        public double price { get; set; }
        public List<StockHistory> history { get; set; }
    }

    internal class StockHistory
    {
        public DateTime when { get; set; }
        public double price { get; set; }
    }
}
