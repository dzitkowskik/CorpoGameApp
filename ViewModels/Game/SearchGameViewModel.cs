using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class SearchGameViewModel
    {
        public int CurrentPlayerId { get; set; }
        public IList<PlayerViewModel> CurrentlyPlayingPlayers { get; set; }
        public IList<PlayerViewModel> QueuedPlayers { get; set; }
        public GameTimeLeftViewModel CurrentGameTimeLeft { get; set; }
        public GameTimeLeftViewModel EstimatedGameTimeLeft { get; set; }
    }
}