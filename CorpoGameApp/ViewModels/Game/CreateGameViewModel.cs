using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class CreateGameViewModel
    {
        public IList<int> FirstTeam { get; set; }
        public IList<int> SecondTeam { get; set; }
    }
}