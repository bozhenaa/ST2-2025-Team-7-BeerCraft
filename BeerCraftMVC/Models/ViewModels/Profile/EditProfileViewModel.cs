using System.ComponentModel.DataAnnotations;

namespace BeerCraftMVC.Models.ViewModels.Profile
{
    public class EditProfileViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string Name { get; set; }
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
}
