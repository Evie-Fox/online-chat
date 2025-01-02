namespace MinimalGameServer.DataStructures
{
    public class ClientRequest
    {
        public ClientRequest(ClientRequestType requestType, Player player, object? content)
        {
            RequestType = requestType;
            Player = player;
            Content = content;
        }

        public ClientRequestType RequestType { get; private set; }
        public Player Player { get; private set; }
        public object? Content { get; private set; }

    }

    public enum ClientRequestType 
    {
        Login = 0,
        Logout = 1,
        Message = 2,
    }
}
