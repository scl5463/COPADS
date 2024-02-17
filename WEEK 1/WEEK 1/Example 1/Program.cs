using System;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var di = Directory.GetFiles(".");
            foreach (var d in di)
            {
                var f = new FileInfo(d);

                //FileSystemInfo.Refresh takes a snapshot of the file from the current file system.
                //Call Refresh before attempting to get the attribute information,
                //or the information will be outdated.


                f.Refresh();
                var fileTime = f.LastWriteTime.ToString("MMM dd HH:mm");
                
               
                System.Console.WriteLine("{0} {1} {2}", f.Length, fileTime, f);
            }   

        }
    }
}