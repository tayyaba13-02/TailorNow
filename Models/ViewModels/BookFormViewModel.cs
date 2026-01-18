using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TailorrNow.Models.ViewModels
{
    public class BookFormViewModel
    {
        [Required]
        [Display(Name = "Tailor")]
        public int TailorId { get; set; }

        [Required]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [Display(Name = "Special Instructions")]
        public string Notes { get; set; } = string.Empty;

      
        [ValidateNever]
        public IEnumerable<Tailor> AvailableTailors { get; set; } = new List<Tailor>();
        
        [ValidateNever]
        public IEnumerable<Service> AvailableServices { get; set; } = new List<Service>();
        
        [ValidateNever]
        public Tailor? SelectedTailor { get; set; }
    }
}