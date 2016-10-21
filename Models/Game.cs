using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CorpoGameApp.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set;}
        public virtual ICollection<PlayerGames> Players { get; set;}
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? WinnersTeam { get; set; }
    }

}