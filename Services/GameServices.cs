using System;
using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Data;
using CorpoGameApp.Models;
using CorpoGameApp.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CorpoGameApp.Services
{
    public class GameServices : IGameServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IOptions<GameSettings> _options;
        private readonly IPlayerServices _playerServices;

        public const int DrawReservedTeamNo = 0;

        public GameServices(
            IPlayerServices playerServices,
            ApplicationDbContext context, 
            IOptions<GameSettings> options)
        {
            _context = context;
            _options = options;
            _playerServices = playerServices;
        }

        public Game CreateGame(IEnumerable<IEnumerable<int>> PlayerTeams)
        {
            var players = new List<PlayerGames>();

            int teamId = DrawReservedTeamNo+1;

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

            using(_context.Database.BeginTransaction())
            {
                if(_context.Games.Any(t => t.EndTime == null))
                    throw new GameAlreadyInProgressException();
                var newGame = _context.Add(game);
                return _context.SaveChanges() > 0 ? newGame.Entity : null;
            }
        }

        public Game GetCurrentGame()
        {
            return _context.Games
                .Include(x => x.Players)
                .FirstOrDefault(t => t.EndTime == null);
        }

        public bool EndGame(int gameId, int? wonTeam)
        {
            var game = _context.Games
                .Include(g => g.Players)
                .ThenInclude(p => p.Player)
                .ThenInclude(x => x.User)
                .Single(t => t.Id == gameId);
            game.EndTime = DateTime.Now;
            game.WinnersTeam = wonTeam??DrawReservedTeamNo;
            
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
            var lastGame = _context.Games
                .Include(x => x.Players)
                .ThenInclude(t => t.Player)
                .ThenInclude(t => t.User)
                .Where(t => t.Players.Any(p => p.PlayerId == playerId))
                .OrderByDescending(t => t.StartTime)
                .FirstOrDefault();
            return lastGame;
        }

        public IQueryable<Game> GetLastNGames(int lastGamesCount)
        {
            return _context.Games
                .Where(t => t.EndTime != null)
                .OrderByDescending(t => t.EndTime)
                .Take(lastGamesCount)
                .Include(t => t.Players)
                .ThenInclude(t => t.Player);
        }
    }
}