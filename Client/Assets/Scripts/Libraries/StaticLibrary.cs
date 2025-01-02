using System;
using System.Linq;
using UnityEngine;

public class StaticLibrary
{
    public class PlayerMessage
    {
        public float TimeSent;
        public string Content;
        public PlayerMessage(float TimeSent, string Content) 
        {
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
        public string JsonContent;

        public ClientRequest(ClientRequestType requestType, Player player, string jsonContent)
        {
            RequestType = requestType;
            Player = player;
            JsonContent = jsonContent;
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
        public string Content;

        public ServerRequest(ServerRequestType requestType, string content = "")
        {
            RequestType = requestType;
            Content = content;
        }
    }

    public enum ServerRequestType
    {
        Ok = 0,
        Error = 1,
    }
}