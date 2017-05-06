using System.Collections.Generic;
using CorpoGameApp.Models;

namespace CorpoGameApp.Services
{
    public interface IPlayerQueueService
    {
        IEnumerable<PlayerQueueItem> GetQueuedPlayers();
        int QueuePlayer(Player player);
        bool Dequeue(PlayerQueueItem item);
    }
}   