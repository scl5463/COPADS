// thread sleeping for 6 seconds...
using System;
using System.Threading;

class Example2
{
    static void Main()
    {

        Thread t = new Thread(PrintNumbersWithDelay);
        t.Start();
        PrintNumbers();
    }
    static void PrintNumbers()
    {
        Console.WriteLine("Starting...");
        for (int c = 1; c < 10; c++)
        {
            Console.WriteLine(c);
        }
    }
    static void PrintNumbersWithDelay()
    {
        Console.WriteLine("Start...");
        for (int b = 1; b < 10; b++)
        {
            Thread.Sleep(TimeSpan.FromSeconds(6));
            Console.WriteLine("Start at " + b );
        }
    }
}
