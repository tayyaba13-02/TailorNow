using Microsoft.EntityFrameworkCore;
using TailorrNow.Models.Interfaces;

namespace TailorrNow.Models.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomTablesContext _context;

        public CustomerRepository(CustomTablesContext context)
        {
            _context = context;
        }

        public Customer GetCustomerByUserId(string userId)
        {
            return _context.Customers
                .FirstOrDefault(c => c.UserId == userId);
        }

        public Tailor GetTailorById(int id)
        {
            return _context.Tailors
                .Include(t => t.Services)
                .FirstOrDefault(t => t.Id == id);
        }

        public bool UpdateCustomer(Customer customer)
        {
            try
            {
                _context.Customers.Update(customer);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Tailor> GetRecommendedTailors(string customerId)
        {
            return _context.Tailors
                .Include(t => t.Services)
                .Take(5)
                .ToList();
        }

        public IEnumerable<Booking> GetCustomerBookings(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                return new List<Booking>();

            return _context.Bookings
                .Include(b => b.Tailor)
                .Include(b => b.Service)
                .Where(b => b.CustomerId.ToString() == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }

        public IEnumerable<Tailor> SearchTailors(string searchTerm, string serviceFilter)
        {
            var query = _context.Tailors
                .Include(t => t.Services)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t =>
                    t.ShopName.Contains(searchTerm) ||
                    t.City.Contains(searchTerm) ||
                    t.Services.Any(s => s.ServiceName.Contains(searchTerm)));
            }

            if (serviceFilter != "all")
            {
                query = query.Where(t =>
                    t.Services.Any(s => s.ServiceName == serviceFilter));
            }

            return query.ToList(); // Removed OrderByDescending(t => t.Rating)
        }

        public IEnumerable<Service> GetServicesByTailorId(int tailorId)
        {
            return _context.Tailors
                .Where(t => t.Id == tailorId)
                .SelectMany(t => t.Services)
                .ToList();
        }

        public bool CreateBooking(Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}