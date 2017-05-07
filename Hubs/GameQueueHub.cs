using CorpoGameApp.Logic;
using CorpoGameApp.Services;
using CorpoGameApp.ViewModels.Game;
using Microsoft.AspNetCore.SignalR;

namespace CorpoGameApp.Hubs
{
    public class GameQueueHub : Hub
    {
        private readonly IPlayerQueueService _playerQueueService;
        private readonly IPlayerServices _playerServices;
        private readonly IGameLogic _gameLogic;

        public GameQueueHub(
            IGameLogic gameLogic,
            IPlayerQueueService playerQueueService,
            IPlayerServices playerServices)
        {
            this._playerServices = playerServices;
            this._playerQueueService = playerQueueService;
            this._gameLogic = gameLogic;
        }

        public void JoinQueue(int playerId)
        {
            var player = _playerServices.GetPlayerById(playerId);

            _playerQueueService.QueuePlayer(player);

            _gameLogic.UpdateQueuedGames();
            Clients.All.updateTeamQueueList(_gameLogic.GetSearchGameViewModel());
        }

        public void Refresh()
        {
            Clients.All.refreshPage();
        }
    }
}