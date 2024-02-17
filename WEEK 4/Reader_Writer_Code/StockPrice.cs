using System;
using System.Threading;
public class StockPrice
{
    public String name { get; private set; }
    private int price;
    private int readerCount;
    private int writerCount;
    private bool wantToWrite;//binary semaphore
    public StockPrice(String _name, int _price)
    {
        name = _name;
        price = _price;
    }

    public void Price(int? threadID)
    {
        // Acquire read lock.
        lock (this)
        {

            // Condition: There are no writes in progress.
            while (wantToWrite || writerCount > 0)//write may be going on or a signal to want to write
                Monitor.Wait(this);
            readerCount++;
        }

        try
        {
            Console.WriteLine("{0} read price of {1}: {2}", threadID, name, price);
            //return price;
        }
        finally
        {
            // Release read lock.
            lock (this)
            {
                readerCount--;
                Monitor.PulseAll(this);

            }

        }
    }

    public void updatePrice(int? threadID, int amount)
    {
        // Acquire write lock.
        lock (this)
        {
            wantToWrite = true;
            // Condition: There are no reads or writes in progress.
            while (readerCount > 0 || writerCount > 0)
                Monitor.Wait(this);
            writerCount++;
        }

        try
        {
            price += amount;
            Console.WriteLine("Thread: {0} updated Price of {1}: {2}", threadID, name, amount);

            
        }
        finally
        {
            // Release write lock.
            lock (this)
            {
                writerCount--;
                Monitor.PulseAll(this);
                wantToWrite = false;
            }
        }
    }
}