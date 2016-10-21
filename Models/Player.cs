using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CorpoGameApp.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public int Score { get; set; }
        public virtual ICollection<PlayerGames> Games { get; set; }
    }
}