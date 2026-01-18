namespace TailorrNow.Models.Interfaces
{
    public interface IAvailabilityRepository
    {
        IEnumerable<Availability> GetAvailabilitiesByTailorId(int tailorId);
        bool AddAvailability(Availability availability);
        bool RemoveAvailability(int availabilityId);
        bool AddRecurringAvailabilities(int tailorId, DateTime startDate, string timeSlot, string repeatOption, int weeks = 1);
    }
}