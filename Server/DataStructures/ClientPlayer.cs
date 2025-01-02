using System.Net.WebSockets;

namespace MinimalGameServer.DataStructures
{
    public class ClientPlayer
    {
        public Player Player { get; private set; }
        public WebSocket? WebSocket { get; private set; }

        public ClientPlayer(Player player, WebSocket? webSocket = null)
        {
            Player = player;
            WebSocket = webSocket;
        }
    }
}
