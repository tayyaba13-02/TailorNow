using System.Collections.Generic;
using TailorrNow.Models;

namespace TailorrNow.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsersCount { get; set; }
        public int ActiveTailorsCount { get; set; }
        public int TotalBookingsCount { get; set; }
        public decimal TotalCommission { get; set; }
        public string AdminName { get; set; } = string.Empty;
        public List<AdminActivityViewModel> RecentActivities { get; set; } = new();
    }

    public class AdminActivityViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
        public string Icon { get; set; } = "bi-activity";
        public string Color { get; set; } = "primary";
        public string BadgeText { get; set; } = string.Empty;
        public string BadgeClass { get; set; } = string.Empty;
    }
}
