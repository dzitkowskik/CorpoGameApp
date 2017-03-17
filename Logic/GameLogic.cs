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

        public GameLogic(
            IGameServices gameServices,
            IPlayerServices playerServices,
            IOptions<GameSettings> options)
        {
            this._playerServices = playerServices;
            this._gameServices = gameServices;
            this._options = options;
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

        public CurrentGameViewModel GetCurrentGameViewModel(Player player)
        {
            // First situation: User has a game without winner
            var gameWithoutOutcome = GetLastGameWithoutOutcome(player);
            if(gameWithoutOutcome != null) return gameWithoutOutcome;

            // Check if a not ended game exists
            var currentGame = _gameServices.GetCurrentGame();
            if(currentGame == null) return null;

            var endTime = currentGame.StartTime.AddMinutes(_options.Value.GameDuration);
            var timeLeft = endTime - DateTime.Now;

            return new CurrentGameViewModel()
            {
                LastGameStart = currentGame.StartTime,
                TimeLeft = timeLeft,
                CurrentGameLasts = true,
                GameId = currentGame.Id
            };
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

        private CurrentGameViewModel GetLastGameWithoutOutcome(Player player)
        {
            var lastGame = _gameServices.GetPlayerLastGame(player.Id);
            if(lastGame != null && lastGame.WinnersTeam == null)
            {
                var teams = lastGame.Players
                    .Where(p => p.Player != null)
                    .GroupBy(t => t.Team, v => v.Player.ToString())
                    .ToDictionary(x => x.Key, y => (IList<string>)y.ToList());
                return new CurrentGameViewModel()
                {
                    GameId = lastGame.Id,
                    CurrentGameLasts = false,
                    Teams = teams
                };
            }
            return null;
        }
    }
}