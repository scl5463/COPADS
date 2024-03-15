using System;
using System.Threading;
using System.Net.Sockets;


namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var p1 = new Program();
            p1.server();
            while (true)
            {
                Thread.Sleep(1000);
            }
            //System.Console.WriteLine("Usage: dotnet run");
           
        }

        public async void server() 
        {
            try {
                
                var client = new UdpClient(5080);
                for (;;) {
                    Console.WriteLine("Hello");
                    var result = await client.ReceiveAsync();// listens on port number 5080
                    var ep = result.RemoteEndPoint;
                    Console.WriteLine(ep.Address + " "+ ep.Port);
                    var data = DateTime.Now.ToString();
                    var msg = System.Text.Encoding.ASCII.GetBytes(data);
                                      
                    var i = await client.SendAsync(msg, msg.Length, ep);
                    System.Console.WriteLine(i + " bytes sent");
                    Thread.Sleep(10000);
                }
            }
            catch (Exception e)
            {   
                System.Console.WriteLine(e);
                System.Console.WriteLine("UDP Client failed");
            }
   
        }
    }

}
