using System;
using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Models;

namespace CorpoGameApp.ViewModels.Game
{
    public class GameViewModel
    {
        public NewGameViewModel NewGame { get; set; }
        public CurrentGameViewModel CurrentGame { get; set; }
        public PlayerViewModel CurrentPlayer { get; set; }
        public bool IsAnyGameInProgress { get { return CurrentGame != null; } }
    }
}