
using TailorrNow.Models;

namespace TailorrNow.Models.Interfaces
{
    public interface ITailorRepository
    {
        Tailor? GetTailorByUserId(string userId);

    }
}
