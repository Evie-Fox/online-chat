namespace MinimalGameServer.DataStructures
{
    public class ClientRequest
    {
        public ClientRequest(ClientRequestType requestType, Player player, string jsonContent = "")
        {
            RequestType = requestType;
            Player = player;
            JsonContent = jsonContent;
        }

        public ClientRequestType RequestType { get; private set; }
        public Player Player { get; private set; }
        public string JsonContent { get; private set; }

    }

    public enum ClientRequestType 
    {
        Login = 0,
        Logout = 1,
        Message = 2,
    }
}
