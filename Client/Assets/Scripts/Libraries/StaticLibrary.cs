using Newtonsoft.Json;
using System;
using System.Linq;

public class StaticLibrary
{
    #nullable enable
    public class PlayerMessage
    {
        public long TimeSent;
        public string Content;
        public string Author;
        public PlayerMessage()
        {
            TimeSent = DateTimeOffset.UtcNow.Ticks;
            Content = "No content";
            Author = "Unknown";
        }
        public PlayerMessage(Player player,long timeSent, string content) 
        {
            TimeSent = timeSent;
            Content = content;
            Author = player.Name;
        }
        [JsonConstructor]
        public PlayerMessage(string Author, long TimeSent, string Content)
        {
            this.Author = Author;
            this.TimeSent = TimeSent;
            this.Content = Content;
        }
    }
    [Serializable]
    public class Player
    {
        public string Id;
        public string Name;
        public Player(string Name)
        {
            this.Name = Name.Trim();
            this.Id = new string(Name.Where(x => char.IsLetterOrDigit(x)).ToArray()).ToLower();
        }

    }
    public class PlayerNames
    {
        public PlayerNames(string[] names)
        {
            Names = names;
        }

        public string[] Names;
    }
    public class ClientRequest
    {
        public ClientRequestType RequestType;
        public Player Player;
        public object? Content;

        public ClientRequest(ClientRequestType requestType, Player player, object? content)
        {
            RequestType = requestType;
            Player = player;
            Content = content;
        }
    }

    public enum ClientRequestType
    {
        Login = 0,
        Logout = 1,
        Message = 2,
    }
    public class ServerRequest
    {
        public ServerRequestType RequestType;
        public object? Content;

        public ServerRequest(ServerRequestType requestType, object? content = null)
        {
            RequestType = requestType;
            Content = content;
        }
    }

    public enum ServerRequestType
    {
        Ok = 0,
        Error = 1,
        None = 2,
        NewMessage = 3,
        PlayerList = 4,
        LogToggle = 5, //use one value to toggle between login and logout
    }
}
