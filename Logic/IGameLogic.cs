using System.Collections.Generic;
using CorpoGameApp.Models;
using CorpoGameApp.ViewModels.Game;

namespace CorpoGameApp.Logic
{
    public interface IGameLogic
    {
        NewGameViewModel GetNewGameViewModel();
        CurrentGameViewModel GetCurrentGameViewModel(Player player);
        Game CreateGame(IEnumerable<IEnumerable<int>> teams);
        bool EndGame(int gameId, int? winningTeam);
    }
}