
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace ConsoleApplication
{

    public class Stock
    {
        private const int writers = 2;
        private const int readers = 10000000;
        public static void Main(string[] args)
        {
            var sp = new StockPrice("rit", 16);
            var tasks = new List<Task>();// Task means the elements will be run asynchronously
            for (var i = 0; i<writers; i++)
            {
                tasks.Add(Task.Run(() => {
                    while (true)
                    {

                        Random r = new Random();
                        sp.updatePrice(Task.CurrentId, 1);
                        Thread.Sleep(r.Next(500, 1500));// sleep for some random period of time before updating 
                    }
                }));
            }
            for (var i = 0; i<readers; i++)
            {
                tasks.Add(Task.Run(() => {
                    while (true)
                    {
                        sp.Price(Task.CurrentId);
                        Thread.Sleep(50);
                    }
                }));
            }

            //tasks is a list; Good for dynamic add/delete operations
            //list converted to arrays with ToArray().
            //Array needed to free up memory allocated for the unused elements by list
            //  WaitAll is called by the main thread to wait for all of the Task objects
            //      in the array to complete execution before termination. 

            Task.WaitAll(tasks.ToArray());
        }
    }

}