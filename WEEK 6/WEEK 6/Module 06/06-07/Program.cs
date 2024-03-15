using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

            var listener = TcpListener.Create(1313);
            try {
                listener.Start();
            } catch (SocketException) {
                Console.WriteLine("Unable to start the server");
                return;
            }
            for(;;) {
                
                var client = await listener.AcceptTcpClientAsync();
                new Task(async () =>
                {
                    try {
                        var netStream = new StreamWriter(client.GetStream());
                        var data = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        await netStream.WriteLineAsync(data);
                        netStream.Flush();
                        Thread.Sleep(10000);
                        Console.WriteLine("Done sleeping");
                                              
                        netStream.Close();
                    } 
                    catch 
                    {
                        Console.WriteLine("Unable to write data");
                    }
                }).Start();
                
            }
        }
    }

}
