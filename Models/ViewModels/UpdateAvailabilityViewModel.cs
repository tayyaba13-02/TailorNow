using System.ComponentModel.DataAnnotations;

namespace TailorrNow.Models.ViewModels
{
    public class UpdateAvailabilityViewModel
    {
        [Required]
        [Display(Name = "Date")]
        public DateTime AvailableDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "From")]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Please enter a valid time in HH:MM format")]
        public string FromTime { get; set; } = "09:00";

        [Required]
        [Display(Name = "To")]
        [RegularExpression(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Please enter a valid time in HH:MM format")]
        public string ToTime { get; set; } = "17:00";

        [Display(Name = "Repeat")]
        public string RepeatOption { get; set; } = "none";
    }
}
