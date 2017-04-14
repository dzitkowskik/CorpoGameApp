using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class SearchTeamViewModel
    {
        public Queue<PlayerViewModel> CurrentlyPlayingPlayers { get; set; }
        public Queue<PlayerViewModel> QueuedPlayers { get; set; }
        public int CurrentGameTimeLeft { get; set; }
        public int EstimatedGameTimeLeft { get; set; }
    }
}