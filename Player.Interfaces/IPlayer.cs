using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace Player.Interfaces
{
    public interface IPlayer : IActor
    {
        public Task<PlayerState> GetUserInfo(CancellationToken cancellationToken);

        public Task<bool> GivePacks(int count, CancellationToken cancellationToken);

        public Task<bool> BuyPacks(int count, CancellationToken cancellationToken);

        public Task<bool> OpenPacks(int count, CancellationToken cancellationToken);

        public Task<bool> AddStickerToAlbum(List<int> stickerIds, CancellationToken cancellationToken);

        public Task<bool> TradeStickers(int? removeId, int? addId, int coins, CancellationToken cancellationToken);
    }
}
