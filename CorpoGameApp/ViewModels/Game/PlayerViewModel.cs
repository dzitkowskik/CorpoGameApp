using CorpoGameApp.Models;

namespace CorpoGameApp.ViewModels.Game
{
    public class PlayerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Score { get; set; }
        public bool IsCurrent { get; set; }

        public PlayerViewModel()
        {
        }

        public PlayerViewModel(Player player)
        {
            if(player != null)
            { 
                Id = player.Id;
                Name = string.IsNullOrEmpty(player.Name) ? player.User.Email : player.Name;
                Surname = player.Surname;
                Score = player.Score.ToString();
            }
        }
    }
}                   