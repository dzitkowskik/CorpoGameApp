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
        private readonly IPlayerServices _playerServices;

        public PlayerQueueService(
            ApplicationDbContext context,
            IPlayerServices playerServices)
        {
            this._playerServices = playerServices;
            _context = context;
        }

        public IList<PlayerQueueItem> GetQueuedPlayers()
        {
            var queuedEnumId = (int)QueuedItemStateEnum.Queued;

            var queueItems = _context.PlayerQueueItems
                .Include(t => t.Player)
                .Include(t => t.Player.User)
                .Include(t => t.State)
                .ToList();

            return queueItems
                .Where(t => t.State.Id == queuedEnumId)
                .ToList();
        }

        public int QueuePlayer(Player player)
        {
            var item = new PlayerQueueItem()
            {
                Player = player,
                State = GetState(QueuedItemStateEnum.Queued)
            };

            _context.PlayerQueueItems.Add(item);
            return _context.SaveChanges();
        }

        public int UpdateQueuedPlayerState(Player player, QueuedItemStateEnum newState)
        {
            var item = _context.PlayerQueueItems
                .Include(t => t.Player)
                .FirstOrDefault(t => t.Player.Id == player.Id);

            item.State = GetState(newState);

            _context.PlayerQueueItems.Update(item);
            return _context.SaveChanges();
        }

        public QueueItemState GetState(QueuedItemStateEnum stateEnum)
        {
            return _context.QueueItemStates.First(t => t.Id == (int)stateEnum);
        }
    }
}