using System;
using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Models;
using CorpoGameApp.Properties;
using CorpoGameApp.Services;
using CorpoGameApp.ViewModels.Game;
using Hangfire;
using Microsoft.Extensions.Options;

namespace CorpoGameApp.Logic
{
    public class GameLogic : IGameLogic
    {
        private readonly IGameServices _gameServices;
        private readonly IPlayerServices _playerServices;
        private readonly IOptions<GameSettings> _options;
        private readonly IPlayerQueueService _playerQueueService;

        public GameLogic(
            IGameServices gameServices,
            IPlayerQueueService playerQueueService,
            IPlayerServices playerServices,
            IOptions<GameSettings> options)
        {
            this._playerServices = playerServices;
            this._gameServices = gameServices;
            this._options = options;
            this._playerQueueService = playerQueueService;
        }

        public NewGameViewModel GetNewGameViewModel()
        {
            var players = _playerServices.GetAllPlayers();
            var newGameViewModel = new NewGameViewModel(
                players, 
                _options.Value.TeamNumber, 
                _options.Value.TeamSize);
            return newGameViewModel;
        }

        public SearchGameViewModel GetSearchGameViewModel(Player player)
        {
            var result = new SearchGameViewModel();
            result.CurrentPlayerId = player.Id;

            // Check if a not ended game exists
            var currentGame = _gameServices.GetCurrentGame();
            if(currentGame != null) 
            {
                var endTime = currentGame.StartTime.AddMinutes(_options.Value.GameDuration);
                var timeLeft = endTime - DateTime.Now;

                result.CurrentGameTimeLeft = new GameTimeLeftViewModel()
                {
                    Label = "Current game time left",
                    SecondsLeft = (int)timeLeft.TotalSeconds
                };

                result.CurrentlyPlayingPlayers = currentGame.Players
                    .Select(t => new PlayerViewModel(t.Player))
                    .ToList();
            }

            // Get queued players
            var queuedPlayers = _playerQueueService.GetQueuedPlayers();

            if(queuedPlayers != null)
            {
                result.QueuedPlayers = queuedPlayers
                    .Select(t => new PlayerViewModel(t.Player))
                    .ToList();

                result.EstimatedGameTimeLeft = new GameTimeLeftViewModel()
                {
                    Label = "Estimated waiting time left",
                    SecondsLeft = 100
                };
            }

            return result;
        }

        public FinishGameViewModel GetFinishGameViewModel(Player player)
        {
            // User has a game without winner

            var lastGame = _gameServices.GetPlayerLastGame(player.Id);
            if(lastGame != null && lastGame.WinnersTeam == null)
            {
                var teams = lastGame.Players
                    .Where(p => p.Player != null)
                    .GroupBy(t => t.Team, v => v.Player.ToString())
                    .ToDictionary(x => x.Key, y => (IList<string>)y.ToList());
                
                return new FinishGameViewModel()
                {
                    GameId = lastGame.Id,
                    Teams = teams
                };
            }
            return null;
        }

        public Game CreateGame(IEnumerable<IEnumerable<int>> teams)
        {
            var result = _gameServices.CreateGame(teams);
            BackgroundJob.Schedule<IGameLogic>(
                logic => logic.EndGame(result.Id, null),
                TimeSpan.FromMinutes(_options.Value.GameDuration));
            return result;
        }

        public bool EndGame(int gameId, int? winningTeam)
        {
            var result = _gameServices.EndGame(gameId, winningTeam);
            return result;
        }
    }
}