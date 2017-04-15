using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Data;
using CorpoGameApp.Enums;
using CorpoGameApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CorpoGameApp.Services
{
    public class PlayerQueueService : IPlayerQueueService
    {
        private readonly ApplicationDbContext _context;

        public PlayerQueueService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PlayerQueueItem> GetQueuedPlayers()
        {
            return _context.PlayerQueueItems
                .Include(t => t.Player)
                .Include(t => t.State)
                .Where(t => t.State.Id == (int)QueuedItemStateEnum.Queued)
                .ToList();
        }

        public int QueuePlayer(int playerId)
        {
            var item = new PlayerQueueItem()
            {
                Player = new Player(){ Id = playerId },
                State = new QueueItemState() { Id = (int)QueuedItemStateEnum.Queued }
            };

            _context.PlayerQueueItems.Add(item);
            return _context.SaveChanges();
        }

        public int UpdateQueuedPlayerState(Player player, QueuedItemStateEnum newState)
        {
            var item = _context.PlayerQueueItems
                .Include(t => t.Player)
                .FirstOrDefault(t => t.Player.Id == player.Id);

            item.State = new QueueItemState() { Id = (int)newState };

            _context.PlayerQueueItems.Update(item);
            return _context.SaveChanges();
        }
    }
}