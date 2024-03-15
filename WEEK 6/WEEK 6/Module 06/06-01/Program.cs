using System;
using System.Net.Sockets;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 1) {
                var host = args[0];
                
                var sock = new Socket(AddressFamily.InterNetwork, 
                    SocketType.Stream, 
                    ProtocolType.Tcp);
                sock.Connect(host, 13);
                byte[] buffer = new byte[1024];
                sock.Receive(buffer);
                var text = System.Text.Encoding.ASCII.GetString(buffer);
                Console.WriteLine(text);
            } else {
                Console.WriteLine("Usage: dotnet run <host>");
            }
            
            

        }
    }

}
