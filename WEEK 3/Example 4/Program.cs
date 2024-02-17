using System;
using System.Threading;


namespace Example4
{
   class GFG
    {

        // Main method
        static void Main(string[] args)
        {
            // Creating and initializing thread
            Thread thr = new Thread(mythread);

            // Name of the thread is Mythread
            thr.Name = "Mythread";
            thr.IsBackground = true;
            thr.Start();

            // IsBackground is the property of Thread
            
            

            Console.WriteLine("Main Thread Ends!!");
        }

        // Static method
        static void mythread()
        {

            // Display the name of the
            // current working thread
            Console.WriteLine("In progress thread is: {0}",
                                Thread.CurrentThread.Name);

            Thread.Sleep(2000);

            Console.WriteLine("Completed thread is: {0}",
                              Thread.CurrentThread.Name);
        }
    }
}
