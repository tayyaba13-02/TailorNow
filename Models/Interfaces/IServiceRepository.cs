
namespace TailorrNow.Models.Interfaces
{
    public interface IServiceRepository
    {
        List<Service> GetServicesByTailorId(int tailorId);
        Service? GetServiceById(int id);
        bool CreateService(Service service);
        bool UpdateService(Service service);
        bool DeleteService(int id);
    }
}