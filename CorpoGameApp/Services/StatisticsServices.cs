using System;
using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Data;
using CorpoGameApp.Properties;
using CorpoGameApp.ViewModels.Game;
using Microsoft.Extensions.Options;

namespace CorpoGameApp.Services
{
    public class StatisticsServices : IStatisticsServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IOptions<GameSettings> _options;
        private readonly IPlayerServices _playerServices;
        private readonly IGameServices _gameServices;

        public StatisticsServices(
            IPlayerServices playerServices,
            IGameServices gameServices,
            ApplicationDbContext context, 
            IOptions<GameSettings> options)
        {
            _context = context;
            _options = options;
            _gameServices = gameServices;
            _playerServices = playerServices;
        }

        public StatisticsViewModel GetTopPlayersStatistic()
        {
            var topN = _options.Value.NumberOfTopPlayersInStatistics;

            var result = new StatisticsViewModel()
            {
                Name = $"Top {topN}",
                Description = $"Results of {topN} best players",
                Headers = new List<string>()
                {
                    "No",
                    "Player",
                    "Score",
                    "Wins",
                    "Defeats",
                    "Draws",
                    "Total"
                },
                Values = new List<IEnumerable<string>>()
            };

            var topPlayers = _playerServices.GetTopPlayers(topN);

            var no = 1;
            foreach(var player in topPlayers)
            {
                var games = player.Games.ToList();
                var wins = games.Count(t => t.Game.WinnersTeam == t.Team);
                var draws = games.Count(t => t.Game.WinnersTeam == GameServices.DrawReservedTeamNo);
                var defeats = games.Count - wins - draws;

                result.Values.Add(new List<string>(){
                    no++.ToString(),
                    player.ToString(),
                    player.Score.ToString(),
                    wins.ToString(),
                    defeats.ToString(),
                    draws.ToString(),
                    games.Count.ToString()
                });
            }

            return result;
        }

        public StatisticsViewModel GetLastGamesStatistic()
        {
            var lastGamesCnt = _options.Value.NumberOfLastGamesInStatistics;

            var result = new StatisticsViewModel()
            {
                Name = $"Recent games",
                Description = $"Results of last {lastGamesCnt} accomplished games",
                Headers = new List<string>()
                {
                    "No",
                    "Teams",
                    "Result",
                    "Date"
                },
                Values = new List<IEnumerable<string>>() 
            };

            var lastGames = _gameServices.GetLastNGames(lastGamesCnt).ToList();

            var no = 1;
            foreach(var game in lastGames)
            {
                var teams = game.Players
                    .GroupBy(t => t.Team)
                    .Select(t => $"{t.Key}:{string.Join(",", t.Select(x => x.Player.ToString()))}");

                var whoWon = game.WinnersTeam.Equals(GameServices.DrawReservedTeamNo) 
                    ? "draw" 
                    : $"Team {game.WinnersTeam} won";

                result.Values.Add(new List<string>()
                {
                    no++.ToString(),
                    string.Join($" vs ", teams),
                    whoWon,
                    game.EndTime.HasValue ? game.EndTime.Value.ToLocalTime().ToString() : "-"
                });
            }

            return result;
        }
    }
}