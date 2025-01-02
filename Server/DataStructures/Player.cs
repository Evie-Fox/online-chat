using System.Net.WebSockets;

namespace MinimalGameServer.DataStructures
{
    public class Player
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public Player(string Name)
        {
            this.Name = Name.Trim(); 
            this.Id = new string(Name.Where(x => char.IsLetterOrDigit(x)).ToArray()).ToLower();
        }
    }
}
