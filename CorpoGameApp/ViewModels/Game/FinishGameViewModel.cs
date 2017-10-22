using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class FinishGameViewModel
    {
        public int GameId { get; set; }
        public int? WinningTeam { get; set; }
        public IDictionary<int, IList<string>> Teams { get; set; }
    }
}