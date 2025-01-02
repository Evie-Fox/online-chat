namespace MinimalGameServer.DataStructures
{
    public class PlayerMessage
    {
        public float TimeSent { get; set; }
        public string Content { get; set; }
        public PlayerMessage(float timeSent, string content)
        {
            TimeSent = timeSent;
            Content = content;
        }
    }

    public class PlayerNames
    {
        public PlayerNames(string[] names)
        {
            Names = names;
        }

        public string[] Names { get; set; }
    }
}
