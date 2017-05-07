using System.Collections.Generic;
using CorpoGameApp.Models;
using CorpoGameApp.ViewModels.Game;

namespace CorpoGameApp.Logic
{
    public interface IGameLogic
    {
        NewGameViewModel GetNewGameViewModel();
        FinishGameViewModel GetFinishGameViewModel(Player player);
        SearchGameViewModel GetSearchGameViewModel(Player player = null);
        Game CreateGame(IEnumerable<IEnumerable<int>> teams);
        bool EndGame(int gameId, int? winningTeam);
        void UpdateQueuedGames(bool refreshClients = false);
    }
}