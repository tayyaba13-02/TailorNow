namespace TailorrNow.Models.Interfaces
{
    public interface ICustomerRepository
    {
        Customer GetCustomerByUserId(string userId);
        bool UpdateCustomer(Customer customer);
        Tailor GetTailorById(int id);
        IEnumerable<Tailor> GetRecommendedTailors(string customerId);
        IEnumerable<Booking> GetCustomerBookings(string customerId);
        IEnumerable<Tailor> SearchTailors(string searchTerm, string serviceFilter);
        IEnumerable<Service> GetServicesByTailorId(int tailorId);
        bool CreateBooking(Booking booking);
    }
}