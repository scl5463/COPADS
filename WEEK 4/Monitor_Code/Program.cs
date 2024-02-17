using System;
using System.Threading;

namespace odd_even_sequence
{
    class Program
    {
        // upto the limit numbers will be printed.
        const int numberLimit = 10;

        static object lck = new object();//lock variable

        static void Main(string[] args)
        {
            Thread oddThread = new Thread(Odd);
            Thread evenThread = new Thread(Even);


            //Start even thread.
            evenThread.Start();
            
            //foreground thread pauses for 100 ms, to make sure even_thread has started
            //or else odd thread may start first resulting other sequence.
            
            Thread.Sleep(100);

            //Start odd thread.
            oddThread.Start();
            


            evenThread.Join();

            Console.WriteLine("\nPrinting done!!!");
        }

        //printing of Odd numbers
        static void Odd()
        {
            try
            {
                //hold lock as console is shared between threads.
                Monitor.Enter(lck);
                for (int i = 1; i <= numberLimit; i = i + 2)
                {
                    //Complete the task ( printing odd number on console)
                    Console.Write(" " + i);
                    //Notify other thread i.e. eventhread
                    //that I'm done you do your job
                    Monitor.Pulse(lck);

                    //I will wait here till even thread notify me
                    // Monitor.Wait(monitor);

                    // without this logic application will wait forever
                    bool isLast = i == numberLimit - 1;
                    if (!isLast)//if false
                        Monitor.Wait(lck); //I will wait here till even thread notify me
                }
            }
            finally
            {
                //Release lock
                Monitor.Exit(lck);
            }
        }

        //printing of even numbers
        static void Even()
        {
            try
            {
                //hold the lock and enter critical region
                Monitor.Enter(lck);
                for (int i = 0; i <= numberLimit; i = i + 2)
                {
                    //Complete the task ( printing even number on console)
                    Console.Write(" " + i);
                    //Notify other thread- here odd thread
                    //that I'm done, you do your job

                    Monitor.Pulse(lck);
                    
                    //I will wait here till odd thread notify me
                    // Monitor.Wait(monitor);

                    bool isLast = i == numberLimit;
                    if (!isLast)
                    {
                        Monitor.Wait(lck);
                    }
                        
                        
                }
            }
            finally
            {
                Monitor.Exit(lck);//release the lock
            }

        }
    }
}