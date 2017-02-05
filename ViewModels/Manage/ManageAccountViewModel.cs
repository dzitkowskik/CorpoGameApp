using System.ComponentModel.DataAnnotations;

namespace CorpoGameApp.ViewModels.Manage
{
    public class ManageAccountViewModel
    {
        [Required]      
        public string Name { get; set; }

        [Required]        
        public string Surname { get; set; }

        [Required]       
        [Display(Name = "Email address")] 
        public string Email { get; set; }

        public bool HasPassword { get; set; }
    }
}
