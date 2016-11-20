using System.Collections.Generic;
using CorpoGameApp.Models;

namespace CorpoGameApp.Services
{
    public interface IGameServices
    {
        bool CreateGame(Dictionary<Player, int> PlayerTeams);
        bool EndGame(int gameId, int? wonTeam);
    }
}