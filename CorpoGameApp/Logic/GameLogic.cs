using System;
using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Hubs;
using CorpoGameApp.Models;
using CorpoGameApp.Properties;
using CorpoGameApp.Services;
using CorpoGameApp.ViewModels.Game;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace CorpoGameApp.Logic
{
    public class GameLogic : IGameLogic
    {
        private readonly IGameServices _gameServices;
        private readonly IPlayerServices _playerServices;
        private readonly IOptions<GameSettings> _options;
        private readonly IPlayerQueueService _playerQueueService;
        private readonly IHubContext<GameQueueHub> _gameQueueHubContext;

        public GameLogic(
            IGameServices gameServices,
            IPlayerQueueService playerQueueService,
            IPlayerServices playerServices,
            IOptions<GameSettings> options,
            IHubContext<GameQueueHub> gameQueueHubContext)
        {
            this._playerServices = playerServices;
            this._gameServices = gameServices;
            this._options = options;
            this._playerQueueService = playerQueueService;
            this._gameQueueHubContext = gameQueueHubContext;
        }

        public NewGameViewModel GetNewGameViewModel()
        {
            // do not return new game view model if current game is in progress
            var currentGame = _gameServices.GetCurrentGame();
            if(currentGame != null) return null;

            var players = _playerServices.GetAllPlayers();
            var newGameViewModel = new NewGameViewModel(
                players, 
                _options.Value.TeamNumber, 
                _options.Value.TeamSize);

            newGameViewModel.Label = "Create a new game";
            return newGameViewModel;
        }

        public SearchGameViewModel GetSearchGameViewModel(Player player = null)
        {
            var result = new SearchGameViewModel();
           
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
            }

            result.GameDurationInSeconds = (int)(
                    new TimeSpan(0, _options.Value.GameDuration, 0)
                    .TotalSeconds);
            result.GameCapacity = _options.Value.TeamSize * _options.Value.TeamNumber;

            result.Label = "Search for team";

            // set current player
            if(player != null)
            {
                result.CurrentPlayerId = player.Id;
                if(result.CurrentlyPlayingPlayers != null)
                    foreach(var p in result.CurrentlyPlayingPlayers)
                        p.IsCurrent = p.Id == player.Id;
                if(result.QueuedPlayers != null)
                    foreach(var p in result.QueuedPlayers)
                        p.IsCurrent = p.Id == player.Id;
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

        public void UpdateQueuedGames(bool refreshClients = false)
        {
            var requiredPlayersNo = _options.Value.TeamNumber * _options.Value.TeamSize;
            var queuedPlayers = _playerQueueService.GetQueuedPlayers();

            if(_gameServices.GetCurrentGame() == null && queuedPlayers.Count() >= requiredPlayersNo)
            {
                // create a game
                var selectedPlayerQueueItems = queuedPlayers.Take(requiredPlayersNo);
                var gameCreated = CreateGame(SelectRandomTeams(selectedPlayerQueueItems.Select(t => t.Player)));

                // dequeue players
                foreach(var queueItem in selectedPlayerQueueItems)
                    _playerQueueService.Dequeue(queueItem);
            }

            if(refreshClients)
            {
                var hubContext = _gameQueueHubContext;
                hubContext.Clients.All.InvokeAsync("refreshPage");
            }
        }

        public Game CreateGame(IEnumerable<IEnumerable<int>> teams)
        {
            var result = _gameServices.CreateGame(teams);
            UpdateQueuedGames(true);
            BackgroundJob.Schedule<IGameLogic>(
                logic => logic.EndGame(result.Id, null),
                TimeSpan.FromMinutes(_options.Value.GameDuration));
            return result;
        }

        public bool EndGame(int gameId, int? winningTeam)
        {
            var result = _gameServices.EndGame(gameId, winningTeam);
            UpdateQueuedGames(true);
            return result;
        }

        private List<List<int>> SelectRandomTeams(IEnumerable<Player> players)
        {
            var teams = new List<List<int>>();
            var list = players.ToList();

            var teamsCount = _options.Value.TeamNumber;
            var teamsSize = _options.Value.TeamSize;

            if(list.Count != teamsCount * teamsSize)
                throw new Exception("Team size does not match");

            var rand = new Random();
            
            Func<int> getPlayerId = () =>
            {
                var selectedPlayerIdx = rand.Next() % list.Count;
                var player = list[selectedPlayerIdx];
                list.Remove(player);
                return player.Id;
            };

            for(int teamIdx = 0; teamIdx < teamsCount; teamIdx++)
            {
                teams.Insert(teamIdx, new List<int>());
                for(int i = 0; i < teamsSize; i++)
                    teams[teamIdx].Add(getPlayerId());
            }

            return teams;
        }
    }
}