using System.Fabric;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace TradingService
{
    internal sealed class TradingService : StatefulService, ITradingService
    {
        private readonly ILogger<TradingService> logger;

        public TradingService(StatefulServiceContext context)
            : base(context)
        {
            logger = new LoggerFactory().CreateLogger<TradingService>();
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        public async Task CreateNewTrade(string playerId, int? offeredStickerId, int? wantedStickerId, int coins)
        {
            var trades = await StateManager.GetOrAddAsync<IReliableDictionary<string, StickerTrade>>("trades");

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var newTrade = new StickerTrade(playerId, offeredStickerId, wantedStickerId, coins);

                await trades.AddAsync(tx, newTrade.Id, newTrade);
                await tx.CommitAsync();

                logger.LogInformation("Added a new trade with id {id}", newTrade.Id);
            }

            return;
        }

        public async Task<StickerTrade?> CompleteTrade(string tradeId)
        {
            var trades = await StateManager.GetOrAddAsync<IReliableDictionary<string, StickerTrade>>("trades");

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var trade = await trades.TryGetValueAsync(tx, tradeId);

                if (!trade.HasValue)
                {
                    logger.LogInformation("Trade with id {id} not found", tradeId);
                    return null;
                }

                if (!string.IsNullOrEmpty(trade.Value.DateCompleted))
                {
                    logger.LogInformation("Trade with id {id} is already completed", tradeId);
                    return null;
                }

                var updatedTrade = new StickerTrade(trade.Value);
                updatedTrade.DateCompleted = DateTime.UtcNow.ToString("O");

                await trades.SetAsync(tx, updatedTrade.Id, updatedTrade);
                await tx.CommitAsync();

                logger.LogInformation("Completed a trade with id {id}", tradeId);

                return updatedTrade;
            }
        }

        public async Task<StickerTrade?> CancelTrade(string tradeId)
        {
            var trades = await StateManager.GetOrAddAsync<IReliableDictionary<string, StickerTrade>>("trades");

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var trade = await trades.TryGetValueAsync(tx, tradeId);

                if (!trade.HasValue)
                {
                    logger.LogInformation("Trade with id {id} not found", tradeId);
                    return null;
                }

                if (!string.IsNullOrEmpty(trade.Value.DateCompleted))
                {
                    logger.LogInformation("Trade with id {id} is already completed", tradeId);
                    return null;
                }

                await trades.TryRemoveAsync(tx, trade.Value.Id);
                await tx.CommitAsync();

                logger.LogInformation("Cancelled a trade with id {id}", tradeId);

                return trade.Value;
            }
        }

        public async Task<List<StickerTrade>> GetActiveTrades(CancellationToken cancellationToken)
        {
            var trades = await StateManager.GetOrAddAsync<IReliableDictionary<string, StickerTrade>>("trades");

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var list = await trades.CreateEnumerableAsync(tx);

                var enumerator = list.GetAsyncEnumerator();

                var result = new List<StickerTrade>();

                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    if (string.IsNullOrEmpty(enumerator.Current.Value.DateCompleted))
                    {
                        result.Add(enumerator.Current.Value);
                    }
                }

                logger.LogInformation("Completed getting all active trades");

                return result;
            }
        }

        public async Task<List<StickerTrade>> GetRecentyCompletedTradesForUser(string playerId, CancellationToken cancellationToken)
        {
            var trades = await StateManager.GetOrAddAsync<IReliableDictionary<string, StickerTrade>>("trades");

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var list = await trades.CreateEnumerableAsync(tx);

                var enumerator = list.GetAsyncEnumerator();

                var result = new List<StickerTrade>();

                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    var current = enumerator.Current.Value;
                    if (current.PlayerId == playerId && !string.IsNullOrEmpty(current.DateCompleted) && !current.IsRead)
                    {
                        result.Add(enumerator.Current.Value);
                    }
                }

                logger.LogInformation("Completed getting recently completed trades for user {playerId}", playerId);

                return result;
            }
        }

        public async Task MarkTradesAsRead(List<string> tradeIds)
        {
            var trades = await StateManager.GetOrAddAsync<IReliableDictionary<string, StickerTrade>>("trades");

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                foreach (var tradeId in tradeIds)
                {
                    var trade = await trades.TryGetValueAsync(tx, tradeId);

                    if (!trade.HasValue)
                    {
                        logger.LogInformation("Trade with id {id} not found", tradeId);
                        return;
                    }

                    if (trade.Value.IsRead)
                    {
                        logger.LogInformation("Trade with id {id} is already marked as read", tradeId);
                        return;
                    }

                    var updatedTrade = new StickerTrade(trade.Value);
                    updatedTrade.IsRead = true;

                    await trades.SetAsync(tx, updatedTrade.Id, updatedTrade);
                    await tx.CommitAsync();

                    logger.LogInformation("Marked a trade with id {id} as read", tradeId);
                }

                return;
            }
        }
    }
}
