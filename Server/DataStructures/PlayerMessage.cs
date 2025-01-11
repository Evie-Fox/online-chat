using Newtonsoft.Json;

namespace MinimalGameServer.DataStructures
{
    public class PlayerMessage
    {
        public long TimeSent { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public PlayerMessage() 
        {
            TimeSent = -1;
            Content = "No content";
            Author = "Unknown";
        }
        public PlayerMessage(Player player, long timeSent, string content)
        {
            Author = player.Name;
            TimeSent = timeSent;
            Content = content;
        }
        [JsonConstructor]
        public PlayerMessage(string Author, long TimeSent, string Content) 
        {
            this.Author = Author;
            this.TimeSent = TimeSent;
            this.Content = Content;
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
