using MinimalGameServer.DataStructures;
using MinimalGameServer;
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
                    break;

                case ClientRequestType.Logout:
                    break;

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
                msg = JsonConvert.DeserializeObject<PlayerMessage>(JsonConvert.SerializeObject(req.Content)); //Needed to cast it properly, as it was serialized
            }
            catch (Exception)
            {
                Console.WriteLine("ClientRequest content didn't cast properly");
                return new(ServerRequestType.Error, "ClientRequest content didn't cast properly");
            }
            Console.WriteLine($"{TimeSpan.FromSeconds(msg.TimeSent)} {req.Player.Name}: {msg.Content}");
            return new(ServerRequestType.Ok, "Message posted");
        }

        public static async Task<ServerRequest> LogIn(ClientPlayer client)
        {
            if (client == null)
            { throw new ArgumentNullException("Player was null"); }

            if (await ServerActions.IsPlayerOnline(client.Player))
            {
                Console.WriteLine($"ID is taken. ID:{client.Player.Id}");
                return new(ServerRequestType.Error, "ID is already in use");
            }

            ServerData.PlayerDict.Add(client.Player.Id, client);
            Console.WriteLine($"Logged in user \"{client.Player.Name}\"");
            return new(ServerRequestType.Ok, "Login successful");
        }

        public static async Task<PlayerNames> PlayersOnline()
        {
            ClientPlayer[] clients = ServerData.PlayerDict.Values.ToArray();
            int count = clients.Count();
            string[] names = new string[count];

            //should this be sorted?

            for (int i = 0; i < count; i++)
            {
                names[i] = clients[i].Player.Name;
            }
            return new PlayerNames(names);
        }

        public static async Task LogOut(Player player)
        {
            await ServerActions.DisconnectPlayer(player.Id);
        }
    }
}