using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace ConsoleApplication
{
    public class Program
    {
        public static Object myLock;
        public static void Main(string[] args)
        {
            var p1 = new Program();
            p1.listen();
            Console.ReadKey();
        }

        public async void listen ()
        {
            TcpListener listener = null;
            try
            {
                 listener = new TcpListener(IPAddress.Any, 1313);
                 listener.Start();
            } catch  {
                Console.WriteLine("Unable to start the server");
                return;
            }
            for(;;) {
                
                var client = await listener.AcceptTcpClientAsync();
                new Task(async () =>
                {
                    NetworkStream netStream = null;
                    try
                    {
                        netStream = client.GetStream();
                        var data = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        var msg = System.Text.Encoding.ASCII.GetBytes(data);
                        await netStream.WriteAsync(msg, 0, msg.Length);
                        Thread.Sleep(1000);
                        Console.WriteLine("Done sleeping");

                    }
                    catch
                    {
                        Console.WriteLine("Unable to write data");
                    }
                    finally
                    {
                        netStream.Close();
                    }
                }).Start();
                
            }
        }
    }

}
