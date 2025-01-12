using MinimalGameServer.DataStructures;
using System.Net.WebSockets;

namespace MinimalGameServer
{
    public static class ServerData
    {
        public static Dictionary<string, ClientPlayer> PlayerDict = new();
        public static Dictionary<WebSocket,  string> WebSocketDict = new();

        public static ClientPlayer GetClientFromWs(WebSocket ws)
        {
            if (WebSocketDict.TryGetValue(ws, out string id))
            {
                if (string.IsNullOrEmpty(id))
                {
                    Console.WriteLine("ID from WS was empty");
                    return null;
                }

                if (PlayerDict.TryGetValue(id, out ClientPlayer player)) 
                {
                    return player;
                }
                Console.WriteLine("Client from ID was not found in PlayerDict");
                return null;
            }
            Console.WriteLine("No related ID found");
            return null;
        }
    }
}
