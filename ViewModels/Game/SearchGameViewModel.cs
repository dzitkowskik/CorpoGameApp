using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class SearchGameViewModel
    {
        public Queue<PlayerViewModel> CurrentlyPlayingPlayers { get; set; }
        public Queue<PlayerViewModel> QueuedPlayers { get; set; }
        public GameTimeLeftViewModel CurrentGameTimeLeft { get; set; }
        public GameTimeLeftViewModel EstimatedGameTimeLeft { get; set; }
    }
}