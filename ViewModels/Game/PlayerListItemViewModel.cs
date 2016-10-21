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
            var userNameParts = player.User.UserName.Split(' ');
            Name = (userNameParts?.Length??0) > 0 ? userNameParts[0] : string.Empty;
            Surname = (userNameParts?.Length??0) > 1 ? userNameParts[1] : string.Empty;
        }
    }
}