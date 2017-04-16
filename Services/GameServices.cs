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

            using(var transaction = _context.Database.BeginTransaction())
            {
                if(_context.Games.Any(t => t.EndTime == null))
                    throw new GameAlreadyInProgressException();
                var newGame = _context.Add(game);
                var result = _context.SaveChanges() > 0 ? newGame.Entity : null;
                transaction.Commit();
                return result;
            }
        }

        public IQueryable<Game> GetAllGames()
        {
            return _context.Games
                .Include(g => g.Players)
                .ThenInclude(p => p.Player)
                .ThenInclude(x => x.User);
        }

        public Game GetCurrentGame()
        {
            return GetAllGames()
                .FirstOrDefault(t => t.EndTime == null);
        }

        public bool EndGame(int gameId, int? wonTeam)
        {
            var game = GetAllGames()
                .Single(t => t.Id == gameId);
            game.EndTime = DateTime.Now;

            if(wonTeam.HasValue)
            {
                game.WinnersTeam = wonTeam.Value;
                foreach(var player in game.Players)
                {
                    if(!wonTeam.HasValue)
                        player.Player.Score += _options.Value.PointsForDraw;
                    else if(player.Team == wonTeam.Value)
                        player.Player.Score += _options.Value.PointsForWin;
                    else
                        player.Player.Score += _options.Value.PointsForLose;
                }
            }

            var result = _context.SaveChanges();
            return result > 0;
        }

        public Game GetPlayerLastGame(int playerId)
        {
            var lastGame = GetAllGames()
                .Where(t => t.Players.Any(p => p.PlayerId == playerId))
                .OrderByDescending(t => t.StartTime)
                .FirstOrDefault();
            return lastGame;
        }

        public IQueryable<Game> GetLastNGames(int lastGamesCount)
        {
            return GetAllGames()
                .Where(t => t.EndTime != null)
                .OrderByDescending(t => t.EndTime)
                .Take(lastGamesCount);
        }
    }
}