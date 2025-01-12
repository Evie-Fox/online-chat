using MinimalGameServer.DataStructures;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace MinimalGameServer.Actions
{
    public static class ClientActions
    {
        public static async Task<ServerRequest> ClientAction(string json, WebSocket ws)
        {
            ClientRequest? req;
            try
            {
                req = JsonConvert.DeserializeObject<ClientRequest>(json);
            }
            catch (JsonException x)
            {
                Console.WriteLine("Invalid ClientRequest Json: " + x.Message);
                return new(ServerRequestType.Error, "Invalid ClientRequest Json");
            }

            if (!await ServerActions.IsPlayerOnline(req.Player))
            {
                if (req.RequestType == ClientRequestType.Login)
                {
                    return await ClientActions.LogIn(new ClientPlayer(req.Player, ws));
                }
                return new(ServerRequestType.Error, "Please log in");
            }
            switch (req.RequestType)
            {
                case ClientRequestType.Login:
                    return new(ServerRequestType.Error, "Already logged in");

                case ClientRequestType.Logout:
                    return await LogOut(req.Player);

                case ClientRequestType.Message:
                    return await PostMessage(req);

                default:
                    break;
            }
            Console.WriteLine("Received bad request");
            return new(ServerRequestType.Error, "Something went wrong in ClientAction");
        }

        public static async Task<ServerRequest> PostMessage(ClientRequest req)
        {
            PlayerMessage msg;
            try
            {
                string json = JsonConvert.SerializeObject(req.Content);
                msg = JsonConvert.DeserializeObject<PlayerMessage> (json); //Needed to cast it properly, as it was serialized
            }
            catch (Exception x)
            {
                Console.WriteLine("ClientRequest content didn't cast properly");
                return new(ServerRequestType.Error, "ClientRequest content didn't cast properly");
            }
            await ServerActions.SendToAll(new ServerRequest(ServerRequestType.NewMessage, msg));

            return new(ServerRequestType.Ok, "Message posted");
        }

        public static async Task<ServerRequest> LogIn(ClientPlayer client)
        {
            if (client == null)
            { throw new ArgumentNullException("Player is null"); }

            if (string.IsNullOrWhiteSpace(client.Player.Id))
            {
                return new(ServerRequestType.Error, "Invalid name");
            }

            if (await ServerActions.IsPlayerOnline(client.Player))
            {
                Console.WriteLine($"ID is taken. ID:{client.Player.Id}");
                return new(ServerRequestType.Error, "Name is already in use");
            }

            ServerData.PlayerDict.Add(client.Player.Id, client);
            ServerData.WebSocketDict[client.WebSocket] = client.Player.Id;

            Console.WriteLine($"Logged in user \"{client.Player.Name}\"");

            ServerActions.UpdateOnlinePlayerList();
            return new(ServerRequestType.LogToggle, "Login successful");
        }

        public static async Task<ServerRequest> LogOut(Player player)
        {
            if (player == null)
            { throw new ArgumentNullException("Player is null"); }
            
            if (!ServerData.PlayerDict.TryGetValue(player.Id, out ClientPlayer client))
            {
                return new(ServerRequestType.Error, "Player is not online");
            }
            ServerData.PlayerDict.Remove(player.Id);
            ServerData.WebSocketDict[client.WebSocket] = string.Empty;

            ServerActions.UpdateOnlinePlayerList();
            return new(ServerRequestType.LogToggle, $"Player \"{client.Player.Name}\"was logged out");
        }

        public static async Task<PlayerNames> PlayersOnline()
        {
            ClientPlayer[] clients = ServerData.PlayerDict.Values.ToArray();
            int count = clients.Count();
            string[] names = new string[count];

            for (int i = 0; i < count; i++)
            {
                names[i] = clients[i].Player.Name;
            }
            return new PlayerNames(names);
        }

    }
}