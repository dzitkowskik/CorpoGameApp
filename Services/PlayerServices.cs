using System;
using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Data;
using CorpoGameApp.Models;
using CorpoGameApp.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CorpoGameApp.Services
{
    public class PlayerServices : IPlayerServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IOptions<GameSettings> _options;

        public PlayerServices(
            ApplicationDbContext context, 
            IOptions<GameSettings> options)
        {
            _context = context;
            _options = options;
        }

        public Player GetPlayerById(int playerId)
        {
            return _context.Players.First(t => t.Id == playerId);
        }

        public int GetPlayerScore(int playerId)
        {
            return GetPlayerById(playerId).Score;
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return _context.Players.Include(t => t.User).AsEnumerable();
        }

        public bool PlayerExists(string userId)
        {
            return _context.Players.Any(t => t.User.Id.Equals(userId));
        }

        public Player GetUserPlayer(string userId)
        {
            return _context.Players.Include(p => p.User).FirstOrDefault(p => p.User.Id == userId);
        }

        public void CreatePlayer(Player player)
        {
            _context.Add(player);
            _context.SaveChanges();
        }

        public void UpdatePlayer(Player player)
        {
            var dbPlayer = _context.Players.First(t => t.User.Id == player.User.Id);
            dbPlayer.Name = player.Name;
            dbPlayer.Surname = player.Surname;
            _context.Update(dbPlayer);
            _context.SaveChanges();
        }

        public IQueryable<Player> GetTopPlayers(int noOfTopPlayers)
        {
            return _context.Players
                .Include(player => player.Games)
                    .ThenInclude(game => game.Game)
                .OrderByDescending(t => t.Score)
                .Take(noOfTopPlayers);
        }
    }
}