using Microsoft.ServiceFabric.Services.Remoting;

namespace TradingService
{
    public interface ITradingService : IService
    {
        public Task CreateNewTrade(string playerId, int? offeredStickerId, int? wantedStickerId, int coins);

        public Task<StickerTrade?> CompleteTrade(string tradeId);

        public Task<StickerTrade?> CancelTrade(string tradeId);

        public Task<List<StickerTrade>> GetActiveTrades(CancellationToken cancellationToken);

        public Task<List<StickerTrade>> GetRecentyCompletedTradesForUser(string playerId, CancellationToken cancellationToken);

        public Task MarkTradesAsRead(List<string> tradeId);
    }
}
