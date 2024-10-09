using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Player.Interfaces;

namespace StickerAlbumFrontend.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("player")]
    public class PlayerActorController : Controller
    {
        private readonly ILogger<PlayerActorController> logger;

        public PlayerActorController(ILogger<PlayerActorController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("{username}/userinfo")]
        public async Task<IActionResult> GetUserInfo(string username, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetUserInfo - Sending request for player with username {username}", username);

            var player = ActorProxy.Create<IPlayer>(new ActorId(username), "fabric:/StickerAlbum");
            var userInfo = await player.GetUserInfo(cancellationToken);

            if (userInfo == null)
            {
                logger.LogInformation("GetUserInfo - Failed to get info for player with username {username}", username);
            }
            else
            {
                logger.LogInformation("GetUserInfo - successfully executed for player with username {username}", username);
            }

            return Json(userInfo);
        }

        [HttpPut("{username}/open")]
        public async Task<IActionResult> OpenPacks(string username, [FromQuery] int count, CancellationToken cancellationToken)
        {
            logger.LogInformation("OpenPacks - Sending request to open {count} packs for player with username {username}", count, username);

            var player = ActorProxy.Create<IPlayer>(new ActorId(username), "fabric:/StickerAlbum");
            var response = await player.OpenPacks(count, cancellationToken);

            if (!response)
            {
                logger.LogInformation("OpenPacks - Failed to open packs for player with username {username}", username);
            }
            else
            {
                logger.LogInformation("OpenPacks - successfully executed for player with username {username}", username);
            }

            return Ok(response);
        }

        [HttpPut("{username}/buy")]
        public async Task<IActionResult> BuyPacks(string username, [FromQuery] int count, CancellationToken cancellationToken)
        {
            logger.LogInformation("BuyPacks - Sending request to buy {count} packs for player with username {username}", count, username);

            var player = ActorProxy.Create<IPlayer>(new ActorId(username), "fabric:/StickerAlbum");
            var response = await player.BuyPacks(count, cancellationToken);

            if (!response)
            {
                logger.LogInformation("BuyPacks - Failed to buy packs for player with username {username}", username);
            }
            else
            {
                logger.LogInformation("BuyPacks - successfully executed for player with username {username}", username);
            }

            return Ok(response);
        }

        [HttpPut("{username}/add")]
        public async Task<IActionResult> AddToAlbum(string username, [FromBody] AddToAlbumRequestBody body, CancellationToken cancellationToken)
        {
            logger.LogInformation("AddToAlbum - Sending request to add stickers to album for player with username {username}", username);

            var player = ActorProxy.Create<IPlayer>(new ActorId(username), "fabric:/StickerAlbum");
            var response = await player.AddStickerToAlbum(body.StickerIds, cancellationToken);

            if (!response)
            {
                logger.LogInformation("AddToAlbum - Failed to add sticker to album for player with username {username}", username);
            }
            else
            {
                logger.LogInformation("AddToAlbum - successfully executed for player with username {username}", username);
            }

            return Ok(response);
        }
    }
}
