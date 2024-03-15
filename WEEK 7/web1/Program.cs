using System.Net;
using System.Net.WebSockets;
using System.Text;


class WebsocketServer
{
    private static async Task HandleWebsocketRequest(HttpListenerContext context)
    {
        if (context.Request.IsWebSocketRequest)
        {
            HttpListenerWebSocketContext websocketContext = await context.AcceptWebSocketAsync(null);

            // Websocket connection established, handle incoming messages here
            byte[] buffer = new byte[1024];
            while (websocketContext.WebSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await websocketContext.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Received message: " + message);
            }

            await websocketContext.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
        }
        else
        {
            context.Response.StatusCode = 400; // Bad Request
            context.Response.Close();
        }
    }

    static void Main(string[] args)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();

        Console.WriteLine("Websocket server running...");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            Task.Run(() => HandleWebsocketRequest(context));
        }
    }
}