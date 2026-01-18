using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TailorrNow.Data;
using TailorrNow.Models;
using TailorrNow.Models.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TailorrNow.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly TailorrNow.Models.CustomTablesContext _context;
        private readonly UserManager<TailorrNow.Data.ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            TailorrNow.Models.CustomTablesContext context, 
            UserManager<TailorrNow.Data.ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(string fullName, string email, string password, string role, string status)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, message = "Email, Role, and Password are required." });
            }

            var user = new TailorrNow.Data.ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            // Using the user-provided password
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Assign Role
                if (await _roleManager.RoleExistsAsync(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

                // Create Profile based on role
                if (role == "Tailor")
                {
                    var tailor = new Tailor
                    {
                        UserId = user.Id,
                        ShopName = fullName + "'s Shop",
                        City = "Not specified",
                        ContactNumber = "Not specified"
                    };
                    _context.Tailors.Add(tailor);
                    await _context.SaveChangesAsync();
                }
                else if (role == "Customer")
                {
                    var customer = new Customer
                    {
                        UserId = user.Id,
                        FullName = fullName,
                        Address = "Not specified"
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "User created successfully!" });
            }

            return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return Json(new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                role = roles.FirstOrDefault()
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(string id, string fullName, string email, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return Json(new { success = false, message = "User not found." });

            user.FullName = fullName;
            user.Email = email;
            user.UserName = email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Sync with profiles
                var currentRoles = await _userManager.GetRolesAsync(user);
                var currentRole = currentRoles.FirstOrDefault();

                if (currentRole != role)
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRoleAsync(user, role);
                    // Note: Profile migration (e.g. Customer to Tailor) is not implemented here 
                    // as it involves complex data shifting (bookings, services etc.)
                }

                // Update Name in profiles
                if (role == "Tailor")
                {
                    var tailor = await _context.Tailors.FirstOrDefaultAsync(t => t.UserId == user.Id);
                    if (tailor != null)
                    {
                        tailor.ShopName = fullName + "'s Shop";
                        await _context.SaveChangesAsync();
                    }
                }
                else if (role == "Customer")
                {
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
                    if (customer != null)
                    {
                        customer.FullName = fullName;
                        await _context.SaveChangesAsync();
                    }
                }

                return Json(new { success = true, message = "User updated successfully!" });
            }

            return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return Json(new { success = false, message = "User not found." });

            // Delete associated profile records first
            var tailor = await _context.Tailors.FirstOrDefaultAsync(t => t.UserId == user.Id);
            if (tailor != null) _context.Tailors.Remove(tailor);

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (customer != null) _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "User deleted successfully!" });
            }

            return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        public async Task<IActionResult> AdminDashboard()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var totalTailors = await _context.Tailors.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();
            var totalCommission = await _context.Bookings
                .Where(b => b.Status == "Completed")
                .Include(b => b.Service)
                .SumAsync(b => (b.Service != null ? b.Service.Price : 0) * 0.1m);

            var recentBookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Service)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsersCount = totalUsers,
                ActiveTailorsCount = totalTailors,
                TotalBookingsCount = totalBookings,
                TotalCommission = totalCommission,
                AdminName = currentUser?.FullName ?? "Admin",
                RecentActivities = recentBookings.Select(b => new AdminActivityViewModel
                {
                    Title = $"Booking for {b.Service?.ServiceName ?? "Service"} by {b.Customer?.FullName ?? "Customer"}",
                    Subtitle = $"Status: {b.Status}",
                    TimeAgo = GetTimeAgo(b.BookingDate),
                    Icon = b.Status == "Completed" ? "bi-check-circle" : (b.Status == "Cancelled" ? "bi-x-circle" : "bi-calendar-event"),
                    Color = b.Status == "Completed" ? "success" : (b.Status == "Cancelled" ? "danger" : "primary"),
                    BadgeText = b.Status,
                    BadgeClass = GetStatusBadgeClass(b.Status)
                }).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ManageUsers(string search = null, string role = null)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => (u.FullName != null && u.FullName.Contains(search)) || (u.Email != null && u.Email.Contains(search)) || (u.PhoneNumber != null && u.PhoneNumber.Contains(search)));
            }

            var users = await query.ToListAsync();
            var userRoles = new Dictionary<string, string>();
            var filteredUsers = new List<TailorrNow.Data.ApplicationUser>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "No Role";
                
                if (string.IsNullOrEmpty(role) || userRole.Equals(role, StringComparison.OrdinalIgnoreCase) || (role == "Tailor" && userRole == "Tailor") || (role == "Customer" && userRole == "Customer"))
                {
                    userRoles[user.Id] = userRole;
                    filteredUsers.Add(user);
                }
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.SearchTerm = search;
            ViewBag.RoleFilter = role;
            return View(filteredUsers);
        }

        public async Task<IActionResult> ViewBookings(string status = "all", string customerName = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Tailor)
                .Include(b => b.Service)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(b => b.Status.ToLower() == status.ToLower());
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                query = query.Where(b => b.Customer.FullName.Contains(customerName));
            }

            if (startDate.HasValue)
            {
                query = query.Where(b => b.BookingDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(b => b.BookingDate <= endDate.Value);
            }

            var bookings = await query.OrderByDescending(b => b.BookingDate).ToListAsync();
            ViewBag.StatusFilter = status;
            ViewBag.CustomerName = customerName;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            return View(bookings);
        }

        public async Task<IActionResult> CommissionReports()
        {
            var tailorEarnings = await _context.Bookings
                .Where(b => b.Status == "Completed")
                .Include(b => b.Tailor)
                .Include(b => b.Service)
                .GroupBy(b => b.Tailor)
                .Select(g => new
                {
                    TailorName = g.Key.ShopName,
                    TotalBookings = g.Count(),
                    Commission = g.Sum(b => (b.Service != null ? b.Service.Price : 0) * 0.1m)
                })
                .ToListAsync();

            ViewBag.TailorEarnings = tailorEarnings;
            return View();
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.Now - dateTime;
            if (span.TotalMinutes < 1) return "just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} mins ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} hours ago";
            return $"{(int)span.TotalDays} days ago";
        }

        private string GetStatusBadgeClass(string status)
        {
            return status?.ToLower() switch
            {
                "pending" => "bg-warning text-dark",
                "confirmed" => "bg-success",
                "completed" => "bg-primary",
                "cancelled" => "bg-danger",
                _ => "bg-secondary"
            };
        }
    }
}


