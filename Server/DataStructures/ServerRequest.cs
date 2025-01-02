namespace MinimalGameServer.DataStructures
{
    public class ServerRequest
    {
        public ServerRequest(ServerRequestType requestType, string content = "")
        {
            RequestType = requestType;
            Content = content;
        }

        public ServerRequestType RequestType { get; private set; }
        public string Content { get; private set; }
    }

    public enum ServerRequestType
    {
        Ok = 0,
        Error = 1,
    }
}
