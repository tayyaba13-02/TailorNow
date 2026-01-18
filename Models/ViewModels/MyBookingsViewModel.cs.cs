namespace TailorrNow.Models.ViewModels
{
    public class MyBookingsViewModel
    {
        public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();
        public string StatusFilter { get; set; } = "all";
    }
}