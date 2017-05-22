using System;
using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Data;
using CorpoGameApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CorpoGameApp.Services
{
    public class PlayerQueueService : IPlayerQueueService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPlayerServices _playerServices;
        private readonly ILogger<PlayerQueueService> _logger;

        public PlayerQueueService(
            ApplicationDbContext context,
            IPlayerServices playerServices,
            ILoggerFactory loggerFactory)
        {
            this._playerServices = playerServices;
            _context = context;
            _logger = loggerFactory.CreateLogger<PlayerQueueService>();
        }

        public IEnumerable<PlayerQueueItem> GetQueuedPlayers()
        {
            return _context.PlayerQueueItems
                .Include(t => t.Player)
                .Include(t => t.Player.User)
                .ToList();
        }

        public int QueuePlayer(Player player)
        {
            var item = new PlayerQueueItem()
            {
                Player = player,
                JoinedTime = DateTime.UtcNow
            };
            using(var transaction = _context.Database.BeginTransaction())
            {
                if(GetQueuedPlayers().Any(t => t.Player.Id == player.Id))
                    throw new PlayerAlreadyQeueuedException();
                _context.PlayerQueueItems.Add(item);
                var result = _context.SaveChanges();
                transaction.Commit();
                return result;
            }
        }

        public bool Dequeue(PlayerQueueItem item)
        {
            try
            {
                _context.PlayerQueueItems.Remove(item);
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(
                    new EventId(), 
                    ex, 
                    $"Error while dequeueing player {item.Player.Id}");
                return false;
            }
        }
    }
}