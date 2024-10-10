namespace StickerAlbumFrontend.Controllers
{
    public class SellStickerRequestBody
    {
        public required int StickerId { get; set; }
        public required int Coins { get; set; }
    }
}
