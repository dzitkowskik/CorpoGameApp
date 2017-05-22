using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class GameViewModel
    {
        public NewGameViewModel NewGame { get; set; }
        public PlayerViewModel CurrentPlayer { get; set; }
        public FinishGameViewModel FinishGame { get; set; }
        public SearchGameViewModel SearchGame { get; set; }
        public IEnumerable<StatisticsViewModel> Statistics { get; set; }
    }
}