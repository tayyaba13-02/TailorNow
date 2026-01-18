using TailorrNow.Models.Interfaces;

namespace TailorrNow.Models.Repositories
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly CustomTablesContext _context;

        public AvailabilityRepository(CustomTablesContext context)
        {
            _context = context;
        }

        public IEnumerable<Availability> GetAvailabilitiesByTailorId(int tailorId)
        {
            return _context.Availabilities
                .Where(a => a.TailorId == tailorId)
                .OrderBy(a => a.AvailableDate)
                .ToList();
        }

        public bool AddAvailability(Availability availability)
        {
            try
            {
                _context.Availabilities.Add(availability);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveAvailability(int availabilityId)
        {
            try
            {
                var availability = _context.Availabilities.Find(availabilityId);
                if (availability != null)
                {
                    _context.Availabilities.Remove(availability);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool AddRecurringAvailabilities(int tailorId, DateTime startDate, string timeSlot, string repeatOption, int weeks = 1)
        {
            try
            {
                var availabilities = new List<Availability>();
                var currentDate = startDate;

                switch (repeatOption.ToLower())
                {
                    case "daily":
                        for (int i = 0; i < 7 * weeks; i++)
                        {
                            availabilities.Add(new Availability
                            {
                                TailorId = tailorId,
                                AvailableDate = currentDate.AddDays(i),
                                TimeSlot = timeSlot
                            });
                        }
                        break;

                    case "weekly":
                        for (int w = 0; w < weeks; w++)
                        {
                            availabilities.Add(new Availability
                            {
                                TailorId = tailorId,
                                AvailableDate = currentDate.AddDays(7 * w),
                                TimeSlot = timeSlot
                            });
                        }
                        break;

                    case "weekdays":
                        for (int i = 0; i < 5 * weeks; i++)
                        {
                            var date = currentDate.AddDays(i);
                           
                            if (date.DayOfWeek == DayOfWeek.Saturday)
                            {
                                date = date.AddDays(2);
                                i += 2;
                            }
                            else if (date.DayOfWeek == DayOfWeek.Sunday)
                            {
                                date = date.AddDays(1);
                                i += 1;
                            }

                            availabilities.Add(new Availability
                            {
                                TailorId = tailorId,
                                AvailableDate = date,
                                TimeSlot = timeSlot
                            });
                        }
                        break;

                    default: 
                        availabilities.Add(new Availability
                        {
                            TailorId = tailorId,
                            AvailableDate = currentDate,
                            TimeSlot = timeSlot
                        });
                        break;
                }

                _context.Availabilities.AddRange(availabilities);
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