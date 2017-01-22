using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Models;

namespace CorpoGameApp.Services
{
    public interface IGameServices
    {
        bool CreateGame(IEnumerable<IEnumerable<int>> PlayerTeams);
        bool EndGame(int gameId, int? wonTeam);
        Game GetCurrentGame();
        Game GetPlayerLastGame(int playerId);
        IQueryable<Game> GetLastNGames(int lastGamesCount);
    }
}