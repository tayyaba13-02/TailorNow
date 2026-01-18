using Microsoft.EntityFrameworkCore;
using TailorrNow.Models;
using TailorrNow.Models.Interfaces;

namespace TailorrNow.Models.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly CustomTablesContext _context;

        public ServiceRepository(CustomTablesContext context)
        {
            _context = context;
        }

        public List<Service> GetServicesByTailorId(int tailorId)
        {
            return _context.Services
                .Where(s => s.TailorId == tailorId)
                .OrderBy(s => s.ServiceName)
                .ToList();
        }

        public Service? GetServiceById(int id)
        {
            return _context.Services.Find(id);
        }

        public bool CreateService(Service service)
        {
            try
            {
                _context.Services.Add(service);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateService(Service service)
        {
            try
            {
                _context.Services.Update(service);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteService(int id)
        {
            try
            {
                var service = _context.Services.Find(id);
                if (service == null) return false;

                _context.Services.Remove(service);
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