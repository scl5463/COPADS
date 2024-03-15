using System;
using System.IO;
using System.Net.Sockets;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 1) {
                var host = args[0];
                var p1 = new Program();
                p1.getData(host);
                Console.ReadKey();
            } else {
                Console.WriteLine("Usage: dotnet run <host>");
            }      
        }

        public async void getData(string host) 
        {
            try {
                var client = new TcpClient();
                await client.ConnectAsync(host, 1313);
                NetworkStream stream = client.GetStream();
                var dataStream = new StreamReader(client.GetStream());
                
                Console.WriteLine(await dataStream.ReadToEndAsync());

                //if you want to send packet to the server,
                //var dataStream = new StreamReader(client.GetStream());
                //stream.Write(sendData, 0, sendData.Length);
                //Console.ReadKey();

                dataStream.Close();
                
            
            }
            catch 
            {
                Console.WriteLine("Unable to connect to the server");
            }
   
        }
    }

}
