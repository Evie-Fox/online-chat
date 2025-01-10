using MinimalGameServer.DataStructures;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace MinimalGameServer.Actions
{
    public static class ServerActions
    {
        public static async Task<string> ListPlayers() 
        {
            int count = ServerData.PlayerDict.Count();
            if (count == 0)
            { return "No player are connected"; }

            Player[] players = ServerData.PlayerDict.Values.Select(x => x.Player).ToArray();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                sb.AppendFormat("ID: {0} Name: {1}\n", players[i].Id,players[i].Name);
            }
            return sb.ToString();
        }
        public static async Task DisconnectPlayer(ClientPlayer client)
        {
            ServerData.PlayerDict.Remove(client.Player.Id);
            if (client.WebSocket != null)
            {
                await client.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected by request", CancellationToken.None);
            }
            Console.WriteLine($"Player {client.Player.Name} was removed");
            return;
        }
        public static async Task<bool> IsPlayerOnline(Player player)
        {
            return ServerData.PlayerDict.ContainsKey(player.Id);
        }
        public static async Task WebSocketHandler(WebSocket wb)
        {
            await WebSocketHandler(wb, CancellationToken.None);
        }
        public static async Task WebSocketHandler(WebSocket ws, CancellationToken ct)
        {
            byte bufferByteSize = 4;

            byte[] buffer = new byte[1024 * bufferByteSize];
            WebSocketReceiveResult results = await ws.ReceiveAsync(buffer, ct);

            while (!results.CloseStatus.HasValue) 
            {
                string content = Encoding.UTF8.GetString(buffer);

                ServerRequest req = await ClientActions.ClientAction(content, ws);

                if (ws.CloseStatus.HasValue) { break; }

                if (req.RequestType == ServerRequestType.None)
                {
                    Array.Clear(buffer, 0, 1024 * bufferByteSize);
                    continue;
                }

                byte[] response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req));
                await ws.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, ct);

                Array.Clear(buffer, 0, 1024 * bufferByteSize);

                results = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
            }

            if (ws.State != WebSocketState.Closed)
            {
                Console.WriteLine("Uexpected closer, aborting web socket.");
                ws.Abort();
            }
            Console.WriteLine("WebSocket closed");
        }
        public static async Task SendToAll(ServerRequest req)
        {
            int count = 0;
            foreach (ClientPlayer client in ServerData.PlayerDict.Values) 
            {
                if (client.WebSocket == null) continue;
                await SendWs(req,client.WebSocket);
                count++;
            }
            Console.WriteLine($"Sent ServerRequest to {count} users");
        }
        public static async Task SendWs(ServerRequest req, WebSocket ws)
        {
            string json = JsonConvert.SerializeObject(req);
            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)),WebSocketMessageType.Text,true, CancellationToken.None);
        }
        public static async Task UpdateOnlinePlayerList()
        {
            PlayerNames names = await ClientActions.PlayersOnline();
            ServerRequest req = new (ServerRequestType.PlayerList, names);
            await SendToAll(req);
        }
    }
}
