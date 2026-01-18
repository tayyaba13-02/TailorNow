namespace TailorrNow.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int TailorId { get; set; }
        public int ServiceId { get; set; } 
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string Notes { get; set; } = string.Empty;

       
        public virtual Customer? Customer { get; set; }
        public virtual Tailor? Tailor { get; set; }
        public virtual Service? Service { get; set; } 
    }
}