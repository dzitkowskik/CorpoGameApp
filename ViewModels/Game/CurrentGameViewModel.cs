using System;
using System.Collections.Generic;

namespace CorpoGameApp.ViewModels.Game
{
    public class CurrentGameViewModel
    {
        public DateTime? LastGameStart { get; set; }
        public TimeSpan TimeLeft { get; set; }
        public double SecondsLeft { get { return TimeLeft.TotalSeconds; } }
        public bool CurrentGameLasts { get; set; }
        public int GameId { get; set; }
        public int? WinningTeam { get; set; }
        public IDictionary<int, IList<string>> Teams { get; set; }
    }
}