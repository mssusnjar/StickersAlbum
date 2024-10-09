namespace StickerAlbumFrontend.Controllers
{
    public class CompleteTradeRequestBody
    {
        public required string TradeId { get; set; }
        public required string OwnerPlayerId { get; set; }
        public required string OtherPlayerId { get; set; }
    }
}
