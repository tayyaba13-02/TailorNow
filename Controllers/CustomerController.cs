using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TailorrNow.Models;
using TailorrNow.Models.Interfaces;
using TailorrNow.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TailorrNow.Data;

namespace TailorrNow.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(
            ICustomerRepository customerRepository,
            UserManager<ApplicationUser> userManager)
        {
            _customerRepository = customerRepository;
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var customer = _customerRepository.GetCustomerByUserId(userId);

            var viewModel = new CustomerDashboardViewModel
            {
                Customer = customer,
                RecommendedTailors = customer != null 
                    ? _customerRepository.GetRecommendedTailors(customer.Id.ToString()) 
                    : new List<Tailor>(),
                RecentBookings = customer != null 
                    ? _customerRepository.GetCustomerBookings(customer.Id.ToString()).Take(3) 
                    : new List<Booking>()
            };

            return View(viewModel);
        }



        public IActionResult MyBookings(string status = "all")
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var customer = _customerRepository.GetCustomerByUserId(userId);
            
            var bookings = customer != null 
                ? _customerRepository.GetCustomerBookings(customer.Id.ToString())
                : new List<Booking>().AsEnumerable();

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                bookings = bookings.Where(b => b.Status.ToLower() == status.ToLower());
            }

            var viewModel = new MyBookingsViewModel
            {
                Bookings = bookings,
                StatusFilter = status
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var customer = _customerRepository.GetCustomerByUserId(userId);
            var user = await _userManager.GetUserAsync(User);

            var viewModel = new UpdateCustomerProfileViewModel
            {
                FullName = customer?.FullName ?? "",
                Address = customer?.Address ?? "",
                Email = user?.Email ?? "",
                PhoneNumber = user?.PhoneNumber ?? ""
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateCustomerProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var customer = _customerRepository.GetCustomerByUserId(userId) ?? new Customer();
                var user = await _userManager.GetUserAsync(User);

               
                customer.UserId = userId;
                customer.FullName = model.FullName;
                customer.Address = model.Address;

                
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                await _userManager.UpdateAsync(user);

                var result = _customerRepository.UpdateCustomer(customer);

                if (result)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("UpdateProfile");
                }

                TempData["ErrorMessage"] = "Failed to update profile. Please try again.";
            }

            return View(model);
        }
        public IActionResult BookTailor(string searchTerm, string serviceFilter)
        {
            var viewModel = new BookTailorViewModel
            {
                Tailors = _customerRepository.SearchTailors(searchTerm ?? string.Empty, serviceFilter ?? "all"),
                SearchTerm = searchTerm,
                SelectedService = serviceFilter
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult BookForm(int tailorId)
        {
            var tailor = _customerRepository.GetTailorById(tailorId);
            if (tailor == null) return RedirectToAction("BookTailor");

            var viewModel = new BookFormViewModel
            {
                TailorId = tailorId,
                SelectedTailor = tailor,
                AvailableTailors = new List<Tailor> { tailor },
                AvailableServices = _customerRepository.GetServicesByTailorId(tailorId),
                BookingDate = DateTime.Today
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookForm(BookFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }
                var customer = _customerRepository.GetCustomerByUserId(userId);

                // Auto-create customer if doesn't exist
                if (customer == null)
                {
                    var user = await _userManager.GetUserAsync(User);
                    customer = new Customer
                    {
                        UserId = userId,
                        FullName = user?.FullName ?? user?.Email ?? "Customer",
                        Address = "Not provided"
                    };
                    var created = _customerRepository.UpdateCustomer(customer);
                    if (!created)
                    {
                        TempData["ErrorMessage"] = "Failed to initialize customer profile.";
                        return View(model);
                    }
                }

                var booking = new Booking
                {
                    CustomerId = customer.Id,
                    TailorId = model.TailorId,
                    ServiceId = model.ServiceId,
                    BookingDate = model.BookingDate,
                    Status = "Pending",
                    Notes = model.Notes
                };

                var result = _customerRepository.CreateBooking(booking);

                if (result)
                {
                    TempData["SuccessMessage"] = "Booking created successfully!";
                    return RedirectToAction("MyBookings");
                }

                TempData["ErrorMessage"] = "Failed to create booking. Please try again.";
            }

           
            var tailor = _customerRepository.GetTailorById(model.TailorId);
            model.SelectedTailor = tailor;
            model.AvailableTailors = tailor != null ? new List<Tailor> { tailor } : new List<Tailor>();
            model.AvailableServices = _customerRepository.GetServicesByTailorId(model.TailorId);
            return View(model);
        }


    }
}