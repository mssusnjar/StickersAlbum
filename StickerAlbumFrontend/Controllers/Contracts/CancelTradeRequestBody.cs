namespace StickerAlbumFrontend.Controllers
{
    public class CancelTradeRequestBody
    {
        public required string TradeId { get; set; }
        public required string OwnerPlayerId { get; set; }
    }
}
