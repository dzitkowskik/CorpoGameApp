using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Models;

namespace CorpoGameApp.ViewModels.Game
{
    public class NewGameViewModel
    {
        public IEnumerable<PlayerListItemViewModel> Players { get; set; }
        
        public int TeamCapacity { get; set; }
        public int NumberOfTeams { get; set; }

        public string Label { get; set; }
        public string Error { get; set; }

        public NewGameViewModel(IEnumerable<Player> players, int numberOfTeams, int teamCapacity)
        {
            Players = players.OrderByDescending(t => t.Score).Select(t => new PlayerListItemViewModel(t));
            NumberOfTeams = numberOfTeams;
            TeamCapacity = teamCapacity;
        }
    }
}