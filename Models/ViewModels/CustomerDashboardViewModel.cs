namespace TailorrNow.Models.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public Customer Customer { get; set; }
        public IEnumerable<Tailor> RecommendedTailors { get; set; }
        public IEnumerable<Booking> RecentBookings { get; set; }
    }
}
