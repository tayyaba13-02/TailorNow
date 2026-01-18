
using TailorrNow.Models;

namespace TailorrNow.Models.ViewModels
{
    public class BookingViewModel
    {
        public Booking Booking { get; set; }
        public int BookingCount { get; set; }

        public string CustomerType =>
            BookingCount > 5 ? "Regular Customer" :
            BookingCount > 1 ? "Returning Customer" : "New Customer";
    }
}