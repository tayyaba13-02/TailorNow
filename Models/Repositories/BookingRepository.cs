using Microsoft.EntityFrameworkCore;  
using TailorrNow.Models.Interfaces;
using TailorrNow.Models;

namespace TailorrNow.Models.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly CustomTablesContext _context;

        public BookingRepository(CustomTablesContext context)
        {
            _context = context;
        }

        public int GetTodaysAppointmentsCount(string tailorUserId)
        {
            
            var tailorId = _context.Tailors
                .Where(t => t.UserId == tailorUserId)
                .Select(t => t.Id)
                .FirstOrDefault();

            return _context.Bookings
                .Count(b => b.TailorId == tailorId &&
                       b.BookingDate.Date == DateTime.Today &&
                       b.Status != "Cancelled");
        }

        public int GetPendingRequestsCount(string tailorUserId)
        {
            var tailorId = _context.Tailors
                .Where(t => t.UserId == tailorUserId)
                .Select(t => t.Id)
                .FirstOrDefault();

            return _context.Bookings
                .Count(b => b.TailorId == tailorId && b.Status == "Pending");
        }

        public IEnumerable<Booking> GetFilteredBookings(int tailorId, string status, DateTime? date, string search)
        {
            var query = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Service)
                .Where(b => b.TailorId == tailorId);

           
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(b => b.Status.ToLower() == status.ToLower());
            }

            
            if (date.HasValue)
            {
                query = query.Where(b => b.BookingDate.Date == date.Value.Date);
            }

           
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    b.Customer.FullName.Contains(search) ||
                    b.Service.ServiceName.Contains(search));
            }

            return query.OrderByDescending(b => b.BookingDate).ToList();
        }

        public IEnumerable<Booking> GetBookingsByTailorId(string userId)
        {
            var tailor = _context.Tailors.FirstOrDefault(t => t.UserId == userId);
            if (tailor == null) return new List<Booking>();

            return _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Service)
                .Where(b => b.TailorId == tailor.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }
        

        public Booking? GetBookingWithCustomer(int bookingId)
        {
            return _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Tailor)
                .FirstOrDefault(b => b.Id == bookingId);
        }

        public bool UpdateBookingStatus(int bookingId, string status)
        {
            var booking = _context.Bookings.Find(bookingId);
            if (booking == null) return false;

            booking.Status = status;
            _context.SaveChanges();
            return true;
        }

        public int GetBookingCountByCustomer(int customerId)
        {
            return _context.Bookings.Count(b => b.CustomerId == customerId);
        }
    }
}