using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Player.Interfaces;
using System.Diagnostics;
using System.Fabric;
using Trader.Interfaces;
using TradingService;

namespace Trader
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.None)]
    internal class Trader : Actor, ITrader, IRemindable
    {
        private IActorTimer? tradeTimer;
        private readonly FabricClient fabricClient;

        private const string ReminderName = "WakeUpReminder";
        private const int tradeIntervalInMinutes = 5;
        private const int maxIdleIntervalInMinutes = 30;

        public Trader(ActorService actorService, ActorId actorId, FabricClient fabricClient) 
            : base(actorService, actorId)
        {
            this.fabricClient = fabricClient;
        }

        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"Trader actor {Id} activated.");

            tradeTimer = RegisterTimer(PerformTradeCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(tradeIntervalInMinutes));

            await RegisterReminderAsync(ReminderName, null, TimeSpan.Zero, TimeSpan.FromMinutes(maxIdleIntervalInMinutes));
        }

        protected override Task OnDeactivateAsync()
        {
            if (tradeTimer != null)
            {
                UnregisterTimer(tradeTimer);
            }

            return base.OnDeactivateAsync();
        }

        protected async Task PerformTradeCallback(Object state)
        {
            await PerformTrade(CancellationToken.None);
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == ReminderName)
            {
                await WakeUp();
            }

            return;
        }

        public async Task PerformTrade(CancellationToken cancellationToken)
        {
            var serviceUri = new Uri("fabric:/StickerAlbum/TradingService");
            var random = new Random();

            var partitions = await fabricClient.QueryManager.GetPartitionListAsync(serviceUri);
            var partitionKey = ((Int64RangePartitionInformation)partitions[random.Next(partitions.Count)].PartitionInformation).LowKey;

            var tradingService = ServiceProxy.Create<ITradingService>(serviceUri, new ServicePartitionKey(partitionKey));

            var trades = await tradingService.GetActiveTrades(cancellationToken);
            var trade = trades[random.Next(trades.Count)];

            if (trade != null && string.IsNullOrEmpty(trade.DateCompleted) && trade.Coins <= 5)
            {
                var ownerPlayer = ActorProxy.Create<IPlayer>(new ActorId(trade.PlayerId), "fabric:/StickerAlbum");
                await ownerPlayer.TradeStickers(null, trade.WantedStickerId, Math.Max(-trade.Coins, 0), cancellationToken);

                await tradingService.CompleteTrade(trade.Id);

                ActorEventSource.Current.ActorMessage(this, $"Completed trade with id {trade.Id}");
            }
            else
            {
                ActorEventSource.Current.ActorMessage(this, $"Skipping trade with id {trade?.Id}");
            }
        }

        public Task WakeUp()
        {
            ActorEventSource.Current.ActorMessage(this, $"Waking up trader actor {Id}");

            return Task.CompletedTask;
        }
    }
}
