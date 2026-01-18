namespace TailorrNow.Models
{
    public class Availability
    {
        public int Id { get; set; }
        public int TailorId { get; set; }
        public DateTime AvailableDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
    }
}
