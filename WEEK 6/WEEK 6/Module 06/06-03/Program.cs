using System;
using System.Globalization;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleApplication
{
    public class Ex
    {
        public static void Main(string[] args)
        {
            var p1 = new Ex();
            p1.listen();
            for (var i = 0; i < 1000; i++)
            {
                Thread.Sleep(10000);
                Console.WriteLine(DateTime.Now.ToString());
            }
            Console.ReadKey();
        }
 
        private async void listen () 
        {

            var listener = TcpListener.Create(1313);
            listener.Start();
            for(;;) {
                var clien = await listener.AcceptTcpClientAsync();
                Console.WriteLine(clien.Connected);
                var netSteam = clien.GetStream();
                var data = DateTime.Now.ToString(CultureInfo.InvariantCulture); //strings
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);//bytes
                netSteam.Write(msg, 0, msg.Length);
                netSteam.Close();
            }
        }
    }

}
