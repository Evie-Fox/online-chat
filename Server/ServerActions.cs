using Microsoft.AspNetCore.Http.HttpResults;
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
        public static async Task DisconnectPlayer(string id)
        {
            if (!ServerData.PlayerDict.TryGetValue(id, out ClientPlayer client))
            {
                Console.WriteLine($"Player with id {id} was not found");
                return;
            }
            //Disconnect player or something
            ServerData.PlayerDict.Remove(id);
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

            while (!results.CloseStatus.HasValue)//why this specifically? 
            {
                string content = Encoding.UTF8.GetString(buffer);

                Console.WriteLine(content);

                ServerRequest req = await ClientActions.ClientAction(content, ws);

                //Console.WriteLine("Response: " + req.RequestType.ToString());

                byte[] response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(req));
                await ws.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, ct);//NEED MORE CONTEXT!!!

                Array.Clear(buffer, 0, 1024 * bufferByteSize);

                results = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
            }

            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", ct);
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
    }
}
