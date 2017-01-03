using System;
using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Data;
using CorpoGameApp.Models;
using CorpoGameApp.Properties;
using Microsoft.Extensions.Options;

namespace CorpoGameApp.Services
{
    public class GameServices : IGameServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IOptions<GameSettings> _options;
        private readonly IPlayerServices _playerServices;

        public GameServices(
            IPlayerServices playerServices,
            ApplicationDbContext context, 
            IOptions<GameSettings> options)
        {
            _context = context;
            _options = options;
            _playerServices = playerServices;
        }

        public bool CreateGame(IEnumerable<IEnumerable<int>> PlayerTeams)
        {
            var players = new List<PlayerGames>();

            int teamId = 0;

            foreach(var team in PlayerTeams)
            {
                players.AddRange(team.Select(t => 
                    new PlayerGames(){ 
                        PlayerId = t, 
                        Team = teamId }));
                teamId++;
            }

            var game = new Game() { 
                StartTime = DateTime.Now,
                Players = players
            };

            _context.Add(game);
            return _context.SaveChanges() > 0;
        }

        public Game GetCurrentGame()
        {
            return _context.Games.FirstOrDefault(t => t.EndTime == null);
        }

        public bool EndGame(int gameId, int? wonTeam)
        {
            var game = _context.Games.Single(t => t.Id == gameId);
            game.EndTime = DateTime.Now;
            game.WinnersTeam = wonTeam;
            
            foreach(var player in game.Players)
            {
                if(!wonTeam.HasValue)
                    player.Player.Score += _options.Value.PointsForDraw;
                else if(player.Team == wonTeam.Value)
                    player.Player.Score += _options.Value.PointsForWin;
                else
                    player.Player.Score += _options.Value.PointsForLose;
            }

            game.EndTime = DateTime.Now;
            var result = _context.SaveChanges();
            return result > 0;
        }

        public Game GetPlayerLastGame(int playerId)
        {
            return _context.Games.FirstOrDefault(t => t.Players.Any(p => p.PlayerId == playerId));
        }
    }
}