using CorpoGameApp.Services;
using CorpoGameApp.ViewModels.Game;
using Microsoft.AspNetCore.SignalR;

namespace CorpoGameApp.Hubs
{
    public class GameQueueHub : Hub
    {
        private readonly IPlayerQueueService _playerQueueService;
        private readonly IPlayerServices _playerServices;

        public GameQueueHub(
            IPlayerQueueService playerQueueService,
            IPlayerServices playerServices)
        {
            this._playerServices = playerServices;
            this._playerQueueService = playerQueueService;
        }

        public void JoinQueue(int playerId)
        {
            var player = _playerServices.GetPlayerById(playerId);

            _playerQueueService.QueuePlayer(player);

            Clients.All.playerJoined(new PlayerViewModel(player));
        }

    }
}