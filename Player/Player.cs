using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Player.Interfaces;

namespace Player
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class Player : Actor, IPlayer, IRemindable
    {
        /// <summary>
        /// Initializes a new instance of Player
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public Player(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        private const string PlayerStateKey = "PlayerState";
        private const string ReminderName = "NewPacks";

        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"Player actor {Id} activated.");

            await StateManager.TryAddStateAsync(PlayerStateKey, new PlayerState()
            {
                Username = Id.ToString(),
                Coins = PlayerConstants.InitialCoins
            });

            await RegisterReminderAsync(ReminderName, null, TimeSpan.Zero, TimeSpan.FromMinutes(PlayerConstants.MinutesForNewPacks));
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == ReminderName)
            {
                await GivePacks(PlayerConstants.AddPacksCount, CancellationToken.None);
            }

            return;
        }

        public async Task<PlayerState> GetUserInfo(CancellationToken cancellationToken)
        {
            ActorEventSource.Current.ActorMessage(this, $"Getting UserInfo for actor {Id}");

            return await StateManager.GetStateAsync<PlayerState>(PlayerStateKey, cancellationToken);
        }

        public async Task<bool> GivePacks(int count, CancellationToken cancellationToken)
        {
            ActorEventSource.Current.ActorMessage(this, $"Giving {count} packs to actor {Id}");

            var state = await StateManager.GetStateAsync<PlayerState>(PlayerStateKey);

            if (state == null || state.NewPacksCount >= PlayerConstants.MaxPacksCount)
            {
                return false;
            }

            state.NewPacksCount = Math.Min(state.NewPacksCount + PlayerConstants.AddPacksCount, PlayerConstants.MaxPacksCount);

            if (state.NewPacksCount == PlayerConstants.MaxPacksCount)
            {
                await UnregisterReminderAsync(GetReminder(ReminderName));
            }
            else
            {
                state.NewPacksDateTime = DateTime.UtcNow.AddMinutes(PlayerConstants.MinutesForNewPacks).ToString("O");
            }

            await StateManager.SetStateAsync(PlayerStateKey, state);

            return true;
        }

        public async Task<bool> BuyPacks(int count, CancellationToken cancellationToken)
        {
            ActorEventSource.Current.ActorMessage(this, $"Buying {count} packs to actor {Id}");

            var state = await StateManager.GetStateAsync<PlayerState>(PlayerStateKey);

            if (state == null || state.NewPacksCount + count > PlayerConstants.MaxPacksCount || state.Coins < PlayerConstants.CoinsForPack * count)
            {
                return false;
            }

            state.NewPacksCount = state.NewPacksCount + count;
            state.Coins -= PlayerConstants.CoinsForPack * count;

            await StateManager.SetStateAsync(PlayerStateKey, state);

            return true;
        }

        public async Task<bool> OpenPacks(int count, CancellationToken cancellationToken)
        {
            ActorEventSource.Current.ActorMessage(this, $"Openning {count} packs for actor {Id}");

            var state = await StateManager.GetStateAsync<PlayerState>(PlayerStateKey, cancellationToken);

            if (state == null || state.NewPacksCount < count)
            {
                return false;
            }

            if (state.NewPacksCount == PlayerConstants.MaxPacksCount)
            {
                await RegisterReminderAsync(ReminderName, null, TimeSpan.FromMinutes(PlayerConstants.MinutesForNewPacks), TimeSpan.FromMinutes(PlayerConstants.MinutesForNewPacks));
                if (DateTime.Parse(state.NewPacksDateTime).ToUniversalTime() < DateTime.UtcNow)
                {
                    state.NewPacksDateTime = DateTime.UtcNow.AddMinutes(PlayerConstants.MinutesForNewPacks).ToString("O");
                }
            }

            state.NewPacksCount = state.NewPacksCount - count;
            var random = new Random();

            for (int i = 0; i < count * PlayerConstants.StickersPerPack; i++)
            {
                int stickerId = random.Next(PlayerConstants.TotalStickers) + 1;
                AddStickerToDeck(state.Stickers, stickerId);
            }

            await StateManager.SetStateAsync(PlayerStateKey, state, cancellationToken);

            return true;
        }

        public async Task<bool> AddStickerToAlbum(List<int> stickerIds, CancellationToken cancellationToken)
        {
            ActorEventSource.Current.ActorMessage(this, $"Adding stickers to the album for actor {Id}");

            var state = await StateManager.GetStateAsync<PlayerState>(PlayerStateKey, cancellationToken);

            if (state == null)
            {
                return false;
            }

            foreach (var stickerId in stickerIds)
            {
                ActorEventSource.Current.ActorMessage(this, $"Adding sticker {stickerId} to the album for actor {Id}");

                if (!state.Stickers.ContainsKey(stickerId) || state.Album.Contains(stickerId))
                {
                    continue;
                }

                state.Album.Add(stickerId);
                RemoveStickerFromDeck(state.Stickers, stickerId);
            }

            await StateManager.SetStateAsync(PlayerStateKey, state, cancellationToken);

            return true;
        }

        public async Task<bool> TradeStickers(int? removeStickerId, int? addStickerId, int coins, CancellationToken cancellationToken)
        {
            ActorEventSource.Current.ActorMessage(this, $"Trading sticker {removeStickerId} for sticker {addStickerId} and {coins} coins for actor {Id}");

            var state = await StateManager.GetStateAsync<PlayerState>(PlayerStateKey, cancellationToken);

            if (state == null || state.Coins + coins < 0 || (removeStickerId.HasValue && !state.Stickers.ContainsKey(removeStickerId.Value)))
            {
                return false;
            }

            if (addStickerId.HasValue)
            {
                AddStickerToDeck(state.Stickers, addStickerId.Value);
            }

            if (removeStickerId.HasValue)
            {
                RemoveStickerFromDeck(state.Stickers, removeStickerId.Value);
            }

            state.Coins = state.Coins + coins;

            await StateManager.SetStateAsync(PlayerStateKey, state, cancellationToken);

            return true;
        }

        private void AddStickerToDeck(Dictionary<int, int> stickers, int stickerId)
        {
            var currentCount = stickers.GetValueOrDefault(stickerId, 0);
            stickers[stickerId] = currentCount + 1;
        }

        private void RemoveStickerFromDeck(Dictionary<int, int> stickers, int stickerId) 
        {
            if (stickers[stickerId] == 1)
            {
                stickers.Remove(stickerId);
            }
            else
            {
                stickers[stickerId]--;
            }
        }
    } 
}
