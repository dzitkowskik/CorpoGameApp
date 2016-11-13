using CorpoGameApp.Models;

namespace CorpoGameApp.ViewModels.Game
{
    public class PlayerListItemViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Score { get; set; }

        public PlayerListItemViewModel(Player player)
        {
            Name = string.IsNullOrEmpty(player.Name) ? player.User.Email : player.Name;
            Surname = player.Surname;
            Score = player.Score.ToString();
        }
    }
}