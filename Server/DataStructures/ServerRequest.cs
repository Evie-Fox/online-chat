namespace MinimalGameServer.DataStructures
{
    #nullable enable
    public class ServerRequest
    {
        public ServerRequest(ServerRequestType requestType, object? content = null)
        {
            RequestType = requestType;
            Content = content;
        }

        public ServerRequestType RequestType { get; private set; }
        public object? Content { get; private set; }
    }

    public enum ServerRequestType
    {
        Ok = 0,
        Error = 1,
    }
}
