using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace TradingService
{
    [DataContract]
    public class StickerTrade
    {
        [SetsRequiredMembers]
        public StickerTrade(string playerId, int? offeredStickerId, int? wantedStickerId, int coins)
        {
            Id = Guid.NewGuid().ToString();
            PlayerId = playerId;
            OfferedStickerId = offeredStickerId;
            WantedStickerId = wantedStickerId;
            Coins = coins;
            DateCreated = DateTime.UtcNow.ToString("O");
        }

        [SetsRequiredMembers]
        public StickerTrade(StickerTrade trade) 
        { 
            Id = trade.Id;
            PlayerId = trade.PlayerId;
            OfferedStickerId = trade.OfferedStickerId;
            WantedStickerId = trade.WantedStickerId;
            Coins = trade.Coins;
            DateCreated = trade.DateCreated;
            DateCompleted = trade.DateCompleted;
            IsRead = trade.IsRead;
        }

        [DataMember]
        public required string Id { get; set; } 

        [DataMember]
        public required string PlayerId { get; set; }

        [DataMember]
        public int? OfferedStickerId { get; set; }

        [DataMember]
        public int? WantedStickerId { get; set; }

        [DataMember]
        public int Coins { get; set; }

        [DataMember]
        public string DateCreated { get; set; } = string.Empty;

        [DataMember]
        public string DateCompleted { get; set; } = string.Empty;

        [DataMember]
        public bool IsRead { get; set; }
    }
}
