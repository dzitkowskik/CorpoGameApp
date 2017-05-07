using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class SearchGameViewModel
    {
        public string Label { get; set; }
        public int CurrentPlayerId { get; set; }
        public IList<PlayerViewModel> CurrentlyPlayingPlayers { get; set; }
        public IList<PlayerViewModel> QueuedPlayers { get; set; }
        public GameTimeLeftViewModel CurrentGameTimeLeft { get; set; }
        public int GameDurationInSeconds { get; set; }
        public int GameCapacity { get; internal set; }
    }
}