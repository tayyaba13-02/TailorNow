using System.ComponentModel.DataAnnotations;

namespace TailorrNow.Models.ViewModels
{
    public class UpdateTailorProfileViewModel
    {
        [Required]
        [Display(Name = "Shop Name")]
        public string ShopName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Contact Number")]
        [Phone]
        public string ContactNumber { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}
