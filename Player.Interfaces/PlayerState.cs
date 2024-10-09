using System.Runtime.Serialization;

namespace Player.Interfaces
{
    [DataContract]
    public class PlayerState
    {
        [DataMember]
        public required string Username { get; set; }

        [DataMember]
        public int Coins { get; set; }

        [DataMember]
        public int NewPacksCount { get; set; }

        [DataMember]
        public string NewPacksDateTime { get; set; } = string.Empty;

        [DataMember]
        public SortedSet<int> Album { get; set; } = new SortedSet<int>();

        [DataMember]
        public Dictionary<int, int> Stickers { get; set; } = new Dictionary<int, int>();
    }
}
