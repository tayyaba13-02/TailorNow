namespace TailorrNow.Models
{
    public class Tailor
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}