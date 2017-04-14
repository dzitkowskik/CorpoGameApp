using System.Collections.Generic;
using CorpoGameApp.Enums;
using CorpoGameApp.Models;

namespace CorpoGameApp.Services.Interfaces
{
    public interface IPlayerQueueService
    {
        IList<PlayerQueueItem> GetQueuedPlayers();
        int QueuePlayer(Player player);
        int UpdateQueuedPlayerState(Player player, QueuedItemStateEnum newState);
    }
}