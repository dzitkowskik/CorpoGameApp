using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CorpoGameApp.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserForeignKey { get; set;}
        public virtual ApplicationUser User { get; set; }
        public int Score { get; set; }
        public virtual ICollection<PlayerGames> Games { get; set; }

        public override string ToString()
        {
            if(!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Surname)) 
                return string.Format("{0} {1}", Name, Surname);
            if(!string.IsNullOrEmpty(Surname)) return Surname;
            if(!string.IsNullOrEmpty(Name)) return Name;
            if(User != null && !string.IsNullOrEmpty(User.UserName)) return User.UserName;
            if(User != null && !string.IsNullOrEmpty(User.Email)) return User.Email;
            return string.Empty;
        }
    }
}