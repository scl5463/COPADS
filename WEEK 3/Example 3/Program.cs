using System;
using System.Threading;


namespace example3
{
    class GFG
    {

        // Main method
        static void Main(string[] args)
        {

            // Creating and initializing thread
            Thread thr = new Thread(mythread);
            thr.Start();
            Console.WriteLine("Main Thread Ends!!");
        }

        // Static method
        static void mythread()
        {
            for (int c = 0; c <= 3; c++)
            {

                Console.WriteLine("mythread is in progress!!");
                Thread.Sleep(1000);
            }
            Console.WriteLine("mythread ends!!");
        }
    }

}
