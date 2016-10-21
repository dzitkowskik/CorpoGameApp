using System;
using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Data;
using CorpoGameApp.Models;
using CorpoGameApp.Properties;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CorpoGameApp.Services
{
    public class GameServices : Services.IGameServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IOptions<GameSettings> _options;

        public GameServices(
            ApplicationDbContext context, 
            IOptions<GameSettings> options)
        {
            _context = context;
            _options = options;
        }

        public bool CreateGame(Dictionary<Player, int> PlayerTeams)
        {
            var game = new Game() { 
                StartTime = DateTime.Now,
                Players = PlayerTeams.Select(t => 
                new PlayerGames {
                    PlayerId = t.Key.Id,
                    Team = t.Value
                }).ToList()
            };
            _context.Add(game);
            return _context.SaveChanges() > 0;
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

        public int GetPlayerScore(int playerId)
        {
            return _context.Players.First(t => t.Id == playerId).Score;
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return _context.Players.AsEnumerable();
        }
    }
}