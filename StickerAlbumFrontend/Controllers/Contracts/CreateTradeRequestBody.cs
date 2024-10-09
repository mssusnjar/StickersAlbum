namespace StickerAlbumFrontend.Controllers
{
    public class CreateTradeRequestBody
    {
        public required string PlayerId { get; set; }
        public int? OfferedStickerId { get; set; }
        public int? WantedStickerId { get; set; }
        public int Coins { get; set; }
    }
}
