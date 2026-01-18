using Microsoft.AspNetCore.Mvc;
using TailorrNow.Models.ViewModels;
using TailorrNow.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using TailorrNow.Models.Interfaces;
using TailorrNow.Data;
using Microsoft.AspNetCore.Authorization;
using TailorrNow.Models;
using Microsoft.EntityFrameworkCore;

namespace TailorrNow.Controllers
{
    [Authorize(Roles = "Tailor")]
    public class TailorController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomTablesContext _businessContext;
        private readonly IAvailabilityRepository _availabilityRepository;

        public TailorController(
           IServiceRepository serviceRepository,        
           IBookingRepository bookingRepository,
           UserManager<ApplicationUser> userManager,
           CustomTablesContext businessContext,
            IAvailabilityRepository availabilityRepository)
        {
            _serviceRepository = serviceRepository;     
            _bookingRepository = bookingRepository;
            _userManager = userManager;
            _businessContext = businessContext;
            _availabilityRepository = availabilityRepository;
        }
        public IActionResult TailorDashboard()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var tailor = _businessContext.Tailors
                .FirstOrDefault(t => t.UserId == userId);

            var user = _userManager.GetUserAsync(User).Result;
            var tailorBookings = tailor != null 
                ? _businessContext.Bookings.Include(b => b.Service).Where(b => b.TailorId == tailor.Id).ToList() 
                : new List<Booking>();

            var viewModel = new TailorDashboardViewModel
            {
                Tailor = tailor,
                TailorName = user?.FullName ?? "Tailor",
                TodaysAppointmentsCount = tailor != null ? _bookingRepository.GetTodaysAppointmentsCount(userId) : 0,
                PendingRequestsCount = tailor != null ? _bookingRepository.GetPendingRequestsCount(userId) : 0,
                TotalBookingsCount = tailorBookings.Count,
                TotalEarning = tailorBookings.Where(b => b.Status == "Completed").Sum(b => b.Service?.Price ?? 0),
                RecentBookings = tailor != null 
                                    ? _bookingRepository.GetBookingsByTailorId(userId).Take(5).ToList()
                                    : new List<Booking>()
            };

            return View(viewModel);
        }

        public IActionResult ViewBookings(string status = "all", string date = null, string search = null)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var tailor = _businessContext.Tailors.FirstOrDefault(t => t.UserId == userId);
            
            DateTime? filterDate = string.IsNullOrEmpty(date) ? null : DateTime.Parse(date);
            var bookings = tailor != null 
                ? _bookingRepository.GetFilteredBookings(tailor.Id, status, filterDate, search)
                : new List<Booking>();

            var viewModel = bookings.Select(b => new BookingViewModel
            {
                Booking = b,
                BookingCount = _bookingRepository.GetBookingCountByCustomer(b.CustomerId)
            }).ToList();

            ViewBag.StatusFilter = status;
            ViewBag.DateFilter = date;
            ViewBag.SearchFilter = search;

            return View(viewModel);
        }






        [HttpGet]
        public IActionResult UpdateAvailability()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var tailor = _businessContext.Tailors.FirstOrDefault(t => t.UserId == userId);
            
            var model = new UpdateAvailabilityViewModel();
            var availabilities = tailor != null 
                ? _availabilityRepository.GetAvailabilitiesByTailorId(tailor.Id)
                : new List<Availability>();

            ViewBag.Availabilities = availabilities.Select(a => new
            {
                Date = a.AvailableDate.ToString("ddd, MMM dd"),
                Time = a.TimeSlot,
                a.Id
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateAvailability(UpdateAvailabilityViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var tailor = _businessContext.Tailors.FirstOrDefault(t => t.UserId == userId);
            if (tailor == null)
            {
                var user = _userManager.FindByIdAsync(userId).Result;
                tailor = new Tailor
                {
                    UserId = userId,
                    ShopName = user?.FullName ?? "My Shop",
                    City = "Update Your City",
                    ContactNumber = "0000000000"
                };
                _businessContext.Tailors.Add(tailor);
                _businessContext.SaveChanges();
            }

            if (ModelState.IsValid)
            {
                string timeSlot = $"{model.FromTime} - {model.ToTime}";
                bool result;

                if (model.RepeatOption == "none")
                {
                    result = _availabilityRepository.AddAvailability(new Availability
                    {
                        TailorId = tailor.Id,
                        AvailableDate = model.AvailableDate,
                        TimeSlot = timeSlot
                    });
                }
                else
                {
                    result = _availabilityRepository.AddRecurringAvailabilities(
                        tailor.Id,
                        model.AvailableDate,
                        timeSlot,
                        model.RepeatOption,
                        weeks: 4); 
                }

                if (result)
                {
                    TempData["SuccessMessage"] = "Availability updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update availability. Please try again.";
                }

                return RedirectToAction(nameof(UpdateAvailability));
            }

          
            var availabilities = _availabilityRepository.GetAvailabilitiesByTailorId(tailor.Id);
            ViewBag.Availabilities = availabilities.Select(a => new
            {
                Date = a.AvailableDate.ToString("ddd, MMM dd"),
                Time = a.TimeSlot,
                a.Id
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateBookingStatus([FromBody] UpdateStatusRequest request)
        {
            if (request == null || request.BookingId <= 0 || string.IsNullOrEmpty(request.Status))
            {
                return Json(new { success = false, message = "Invalid request data." });
            }

            try
            {
                var result = _bookingRepository.UpdateBookingStatus(request.BookingId, request.Status);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class UpdateStatusRequest
        {
            public int BookingId { get; set; }
            public string Status { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RemoveAvailability(int id)
        {
            try
            {
                var result = _availabilityRepository.RemoveAvailability(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                
                return Json(new { success = false, error = ex.Message });
            }
        }



        public IActionResult ManageServices()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var tailor = _businessContext.Tailors.FirstOrDefault(t => t.UserId == userId);
            
            var services = tailor != null 
                ? _serviceRepository.GetServicesByTailorId(tailor.Id)
                : new List<Service>();

            var viewModel = new ManageServicesViewModel
            {
                Services = services.Select(s => new ServiceViewModel
                {
                    Id = s.Id,
                    ServiceName = s.ServiceName,
                    Price = s.Price
                }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult AddService()
        {
            return View(new ServiceViewModel());
        }

        [HttpPost]
        public IActionResult AddService(ServiceViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var tailor = _businessContext.Tailors.FirstOrDefault(t => t.UserId == userId);
            if (tailor == null)
            {
                var user = _userManager.FindByIdAsync(userId).Result;
                tailor = new Tailor
                {
                    UserId = userId,
                    ShopName = user?.FullName ?? "My Shop",
                    City = "Update Your City",
                    ContactNumber = "0000000000"
                };
                _businessContext.Tailors.Add(tailor);
                _businessContext.SaveChanges();
            }

            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    ServiceName = model.ServiceName,
                    Price = model.Price,
                    TailorId = tailor.Id
                };

                if (_serviceRepository.CreateService(service))
                {
                    TempData["SuccessMessage"] = "Service added successfully";
                    return RedirectToAction("ManageServices");
                }
                TempData["ErrorMessage"] = "Failed to add service";
            }
            return View(model);
        }

        public IActionResult EditService(int id)
        {
            var service = _serviceRepository.GetServiceById(id);
            if (service == null) return NotFound();

            var viewModel = new ServiceViewModel
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                Price = service.Price
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditService(ServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = _serviceRepository.GetServiceById(model.Id);
                if (service == null) return NotFound();

                service.ServiceName = model.ServiceName;
                service.Price = model.Price;

                if (_serviceRepository.UpdateService(service))
                {
                    TempData["SuccessMessage"] = "Service updated successfully";
                    return RedirectToAction("ManageServices");
                }
                TempData["ErrorMessage"] = "Failed to update service";
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteService(int id)
        {
            if (_serviceRepository.DeleteService(id))
                TempData["SuccessMessage"] = "Service deleted successfully";
            else
                TempData["ErrorMessage"] = "Failed to delete service";

            return RedirectToAction("ManageServices");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var user = await _userManager.FindByIdAsync(userId);
            var tailor = _businessContext.Tailors.FirstOrDefault(t => t.UserId == userId);

            if (tailor == null)
            {
                tailor = new Tailor
                {
                    UserId = userId,
                    ShopName = user?.FullName ?? "My Shop",
                    City = "Update Your City",
                    ContactNumber = user?.PhoneNumber ?? "0000000000"
                };
                _businessContext.Tailors.Add(tailor);
                _businessContext.SaveChanges();
            }

            var viewModel = new UpdateTailorProfileViewModel
            {
                ShopName = tailor.ShopName,
                City = tailor.City,
                ContactNumber = tailor.ContactNumber,
                FullName = user?.FullName ?? "",
                Email = user?.Email ?? ""
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateTailorProfileViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var tailor = _businessContext.Tailors.FirstOrDefault(t => t.UserId == userId);
                if (tailor != null)
                {
                    tailor.ShopName = model.ShopName;
                    tailor.City = model.City;
                    tailor.ContactNumber = model.ContactNumber;

                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        user.FullName = model.FullName;
                        user.PhoneNumber = model.ContactNumber;
                        await _userManager.UpdateAsync(user);
                    }

                    _businessContext.Tailors.Update(tailor);
                    _businessContext.SaveChanges();

                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("UpdateProfile");
                }
            }

            return View(model);
        }
    }
}