using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class SearchTeamViewModel
    {
        public int TeamCount { get; set; }
        public Queue<TeamViewModel> TeamQueue { get; set; }
        public int? SelectedTeamId { get; set; }
    }
}