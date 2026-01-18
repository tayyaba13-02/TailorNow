using System.Collections.Generic;

namespace TailorrNow.Models.ViewModels
{
    public class TailorDashboardViewModel
    {
        public Tailor Tailor { get; set; } = new Tailor(); 
        public string TailorName { get; set; } = string.Empty;
        public int TodaysAppointmentsCount { get; set; }
        public int PendingRequestsCount { get; set; }
        public int TotalBookingsCount { get; set; }
        public decimal TotalEarning { get; set; }
        public List<Booking> RecentBookings { get; set; } = new();
    }
}
