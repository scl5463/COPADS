using System.Net.WebSockets;
using System.Text;


class WebsocketClient
{
    static async Task Main(string[] args)
    {
        ClientWebSocket client = new ClientWebSocket();
        Uri serverUri = new Uri("ws://localhost:8080/");

        await client.ConnectAsync(serverUri, CancellationToken.None);

        // Websocket connection established, handle incoming and outgoing messages here
        int i = 0;
        while (i<4)
        {
            string message = "Hello, server!";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            i++;
        }
        //await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
    }
}