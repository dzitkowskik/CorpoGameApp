using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class GameViewModel
    {
        public NewGameViewModel NewGame { get; set; }
        public CurrentGameViewModel CurrentGame { get; set; }
        public PlayerViewModel CurrentPlayer { get; set; }
        public IEnumerable<StatisticsViewModel> Statistics { get; set; }
        public bool IsAnyGameInProgress { get { return CurrentGame != null; } }
    }
}