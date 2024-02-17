using System;
using System.IO;

class Test
{
    public static void Main()
    {
        string path = @"c:\temp\myTest.txt";
        if (!File.Exists(path))
        {
           //using (StreamWriter sw = File.CreateText(path))
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("Hello");
                sw.WriteLine("and");
                sw.WriteLine("welcome!");

            }
        }
        using (StreamReader sr = File.OpenText(path))
        {
            string s;
            while ((s=sr.ReadLine())!=null)
            {
                Console.WriteLine(s);
            }
        }




    }
}