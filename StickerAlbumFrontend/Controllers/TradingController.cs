using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Player.Interfaces;
using System.Fabric.Query;
using System.Fabric;
using TradingService;

namespace StickerAlbumFrontend.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("trades")]
    public class TradingController : Controller
    {
        private readonly FabricClient fabricClient;
        private readonly ILogger<TradingController> logger;
        private readonly Uri serviceUri;

        public TradingController(FabricClient fabricClient, ILogger<TradingController> logger)
        {
            this.fabricClient = fabricClient;
            this.logger = logger;
            serviceUri = new Uri("fabric:/StickerAlbum/TradingService");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTrade([FromBody] CreateTradeRequestBody body, CancellationToken cancellationToken)
        {
            logger.LogInformation("CreateTrade - Sending request to create new trade for user {username}", body.PlayerId);

            var player = ActorProxy.Create<IPlayer>(new ActorId(body.PlayerId), "fabric:/StickerAlbum");
            var result = await player.TradeStickers(body.OfferedStickerId, null, Math.Min(-body.Coins, 0), cancellationToken);

            if (result)
            {
                var tradingService = GetTradingService(body.PlayerId);
                await tradingService.CreateNewTrade(body.PlayerId, body.OfferedStickerId, body.WantedStickerId, body.Coins);
            }

            logger.LogInformation("CreateTrade - successfully created trade for user {username}", body.PlayerId);

            return Ok();
        }

        [HttpPut("complete")]
        public async Task<IActionResult> CompleteTrade([FromBody] CompleteTradeRequestBody body, CancellationToken cancellationToken)
        {
            logger.LogInformation("CompleteTrade - Sending request to complete trade with {id}", body.TradeId);

            var tradingService = GetTradingService(body.OwnerPlayerId);
            var trade = await tradingService.CompleteTrade(body.TradeId);

            if (trade == null)
            {
                logger.LogInformation("CompleteTrade - trade with {id} not found", body.TradeId);
                return NotFound();
            }

            var ownerPlayer = ActorProxy.Create<IPlayer>(new ActorId(body.OwnerPlayerId), "fabric:/StickerAlbum");
            var otherPlayer = ActorProxy.Create<IPlayer>(new ActorId(body.OtherPlayerId), "fabric:/StickerAlbum");

            await ownerPlayer.TradeStickers(null, trade.WantedStickerId, Math.Max(-trade.Coins, 0), cancellationToken);
            await otherPlayer.TradeStickers(trade.WantedStickerId, trade.OfferedStickerId, trade.Coins, cancellationToken);

            logger.LogInformation("CompleteTrade - successfully completed trade with {id}", body.TradeId);

            return Ok();
        }

        [HttpPut("cancel")]
        public async Task<IActionResult> CancelTrade([FromBody] CancelTradeRequestBody body, CancellationToken cancellationToken)
        {
            logger.LogInformation("CancelTrade - Sending request to cancel trade with {id}", body.TradeId);

            var tradingService = GetTradingService(body.OwnerPlayerId);
            var trade = await tradingService.CancelTrade(body.TradeId);

            if (trade == null)
            {
                logger.LogInformation("CancelTrade - trade with {id} not found", body.TradeId);
                return NotFound();
            }

            var ownerPlayer = ActorProxy.Create<IPlayer>(new ActorId(body.OwnerPlayerId), "fabric:/StickerAlbum");
            await ownerPlayer.TradeStickers(null, trade.OfferedStickerId, Math.Max(trade.Coins, 0), cancellationToken);

            logger.LogInformation("CancelTrade - successfully cancelled trade with {id}", body.TradeId);

            return Ok();
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetTradesList(CancellationToken cancellationToken)
        {
            logger.LogInformation("GetTradesList - Sending request to get all active trades");

            var partitions = await fabricClient.QueryManager.GetPartitionListAsync(serviceUri);

            var result = new List<StickerTrade>();

            foreach (Partition partition in partitions)
            {
                var partitionKey = ((Int64RangePartitionInformation)partition.PartitionInformation).LowKey;
                var tradingService = GetTradingService(partitionKey);

                var response = await tradingService.GetActiveTrades(cancellationToken);

                if (response == null)
                {
                    continue;
                }

                result.AddRange(response);

                logger.LogInformation("GetTradesList - Added trades from partition with key {partitionKey}", partitionKey);
            }

            logger.LogInformation("GetTradesList - Completed getting trades from all partitions");

            return Json(result);
        }

        [HttpGet("{username}/list")]
        public async Task<IActionResult> GetRecentTradesForUser(string username, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetRecentTradesForUser - Getting recently completed trades for user {username}", username);

            var tradingService = GetTradingService(username);

            var response = await tradingService.GetRecentyCompletedTradesForUser(username, cancellationToken);

            logger.LogInformation("GetTradesList - Completed getting recently completed trades for user {username}", username);

            return Json(response);
        }

        [HttpPut("{username}/mark")]
        public async Task<IActionResult> MarkTradesForUser(string username, [FromBody] MarkTradeRequestBody body, CancellationToken cancellationToken)
        {
            logger.LogInformation("MarkTradesForUser - Marking recently completed trades for user {username} as read", username);

            var tradingService = GetTradingService(username);

            await tradingService.MarkTradesAsRead(body.TradeIds);

            logger.LogInformation("MarkTradesForUser - Completed marking recently completed trades for user {username} as read", username);

            return Ok();
        }

        private long GetPartitionKey(string partitionKey)
        {
            return Char.ToUpper(partitionKey.First()) - 'A';
        }

        private ITradingService GetTradingService(string partitionKey)
        {
            return ServiceProxy.Create<ITradingService>(serviceUri, new ServicePartitionKey(GetPartitionKey(partitionKey)));
        }

        private ITradingService GetTradingService(long partitionKey)
        {
            return ServiceProxy.Create<ITradingService>(serviceUri, new ServicePartitionKey(partitionKey));
        }
    }
}
