using System.Collections.Generic;
using CorpoGameApp.Enums;
using CorpoGameApp.Models;

namespace CorpoGameApp.Services
{
    public interface IPlayerQueueService
    {
        IList<PlayerQueueItem> GetQueuedPlayers();
        int QueuePlayer(int playerId);
        int UpdateQueuedPlayerState(Player player, QueuedItemStateEnum newState);
    }
}