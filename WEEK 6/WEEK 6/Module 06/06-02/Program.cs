using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleApplication
{
    public class Ex
    {
        public static void Main(string[] args)
        {
            if (args.Length == 1) {
                var host = args[0];
                var p1 = new Ex();
                p1.getData(host);
                Console.WriteLine("hello");
                Thread.Sleep(1000);
                Console.ReadKey();
                
            } else {
                Console.WriteLine("Usage: dotnet run <host>");
            }      
        }

        public async void getData(string host) 
        {
            var data = new Byte[256];
            var client = new TcpClient();
            await client.ConnectAsync(host, 13);
            var dataStream = client.GetStream();
            Int32 bytes = dataStream.Read(data, 0, data.Length);
            Console.WriteLine(System.Text.Encoding.ASCII.GetString(data, 0, bytes));
        }
    }

}
