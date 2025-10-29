using System.ComponentModel.DataAnnotations;

namespace BeerCraftMVC.Models.ViewModels.Profile
{
    public class ProfileViewModel
    {
        public int UserId { get; set; } 

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Member Since")]
        [DataType(DataType.Date)] 
        public DateTime CreatedAt { get; set; }
    }
}
