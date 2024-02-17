using System;
using System.Diagnostics; //for stopwatch class

namespace ForeachParallel
{
    internal class Program
    {

        static string[] names = new string[] { "John", ",Jakson", "Arthur", "Dan","Raj",
            "Carol","Steve","Mendy", "Alex","Amir", "Ali","Fatma",
            "Cale","Cane", "Zayn","Faust", "Goethe", "Pep","Sydney",
            "Ray" ,"Wayne","Clay", "Thomas", "Thompson","Freya", "Ragnar",
            "Haaland", "Cris", "Robert", "Angela", "Micheal" ,"Pam",
            "Jim", "Trent", "Max", "Lewis"};

        static void Main(string[] args)
        {
            Console.WriteLine("Names count: "+ names.Length);
            var stopwatch = Stopwatch.StartNew();

            foreach (var item in names)
            {
                if (item == "Steve")
                {
                    Console.WriteLine("Found the guy in");
                    Console.WriteLine(stopwatch.Elapsed.TotalSeconds.ToString());
                }
            }
            stopwatch = Stopwatch.StartNew();
            Parallel.ForEach(names, name =>
            {
                if (name == "Steve")
                {
                    Console.WriteLine("Found the guy in ");
                    Console.WriteLine(stopwatch.Elapsed.TotalSeconds.ToString());
                }
            });
            
      
        }

    }
}