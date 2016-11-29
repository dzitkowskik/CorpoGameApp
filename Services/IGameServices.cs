using System.Collections.Generic;
using CorpoGameApp.Models;

namespace CorpoGameApp.Services
{
    public interface IGameServices
    {
        bool CreateGame(IEnumerable<IEnumerable<int>> PlayerTeams);
        bool EndGame(int gameId, int? wonTeam);
    }
}