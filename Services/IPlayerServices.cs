using System.Collections.Generic;
using CorpoGameApp.Models;

namespace CorpoGameApp.Services
{
    public interface IPlayerServices
    {
        int GetPlayerScore(int playerId);
        IEnumerable<Player> GetAllPlayers();
        bool PlayerExists(string userId);
        Player GetUserPlayer(string userId);
        void CreatePlayer(Player player);
        void UpdatePlayer(Player player);
    }
}