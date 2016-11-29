using System;
using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Models;

namespace CorpoGameApp.ViewModels.Game
{
    public class CreateGameViewModel
    {
        public IList<int> FirstTeam { get; set; }
        public IList<int> SecondTeam { get; set; }
    }
}