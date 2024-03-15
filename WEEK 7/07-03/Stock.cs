using System;
using System.Collections.Generic;


namespace ConsoleApplication
{
    internal class Stock
    {
        public string name { get; set; }
        public double price { get; set; }
        public List<StockHistory> history { get; set; } //inheriting and storing objects of a class 
    }

    internal class StockHistory
    {
        public DateTime when { get; set; }
        public double price { get; set; }
    }
}
