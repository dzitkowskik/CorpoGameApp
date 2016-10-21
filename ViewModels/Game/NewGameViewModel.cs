using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Models;

namespace CorpoGameApp.ViewModels.Game
{
    public class NewGameViewModel
    {
        public IEnumerable<PlayerListItemViewModel> Players { get; set; }
        
        private int TeamCapacity { get; set; }
        private int NumberOfTeams { get; set; }

        public NewGameViewModel(IEnumerable<Player> players, int numberOfTeams, int teamCapacity)
        {
            Players = players.Select(t => new PlayerListItemViewModel(t));
            NumberOfTeams = numberOfTeams;
            TeamCapacity = teamCapacity;
        }
    }
}