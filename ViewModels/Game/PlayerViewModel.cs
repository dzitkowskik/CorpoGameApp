using System.Collections.Generic;
using System.Linq;
using CorpoGameApp.Models;

namespace CorpoGameApp.ViewModels.Game
{
    public class PlayerViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public PlayerViewModel()
        {
            
        }

        public PlayerViewModel(Player player)
        {
            if(player != null)
            {
                this.Name = player.Name;
                this.Surname = player.Surname;
            }
        }
    }
}