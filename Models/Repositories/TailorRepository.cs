using TailorrNow.Models;
using System.Linq;
using TailorrNow.Models.Interfaces;

namespace TailorrNow.Models.Repositories
{
    public class TailorRepository : ITailorRepository
    {
        private readonly CustomTablesContext _context;

        public TailorRepository(CustomTablesContext context)
        {
            _context = context;
        }

        public Tailor? GetTailorByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            return _context.Tailors.FirstOrDefault(t => t.UserId == userId);
        }
    }
}
