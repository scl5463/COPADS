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
                System.Console.WriteLine("Usage: dotnet run <host>");
            }      
        }

        public async void getData(string host) 
        {
            try {
                var client = new UdpClient();

                
                byte[] b = new byte[1];//Datagram to be sent. The byte array has only one element.
                // send a request to the server 
                await client.SendAsync(b, 1, host, 5080);
                
                var result = await client.ReceiveAsync();
                var theTime = System.Text.Encoding.ASCII.GetString(result.Buffer);
                System.Console.WriteLine(theTime);
            
            }
            catch (Exception)
            {
               
                System.Console.WriteLine("Unable to connect to the server");
            }
   
        }
    }

}
