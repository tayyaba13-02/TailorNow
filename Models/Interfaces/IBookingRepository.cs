using TailorrNow.Models;

public interface IBookingRepository
{
    IEnumerable<Booking> GetFilteredBookings(int tailorId, string status, DateTime? date, string search);
    int GetTodaysAppointmentsCount(string userId);
    int GetPendingRequestsCount(string userId);
    IEnumerable<Booking> GetBookingsByTailorId(string userId);
    bool UpdateBookingStatus(int bookingId, string status);
    int GetBookingCountByCustomer(int customerId);
}