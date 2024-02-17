using System;
using System.Threading.Tasks;

namespace Example6
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("C# Parallel For Loop");
            /*var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 2
            };
            */


            //Loop will start from 1 until 10
            //Here 1 is the start index which is Inclusive
            //Here 11 us the end index which is Exclusive
            //Here number is similar to i of our standard for loop
            //The value will be store in the variable number
            Parallel.For(1, 11, i =>
            {
                Console.WriteLine(i);
            });
            //Console.ReadLine();
        }
    }
}