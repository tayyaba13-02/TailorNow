using Microsoft.AspNetCore.Mvc;

namespace TailorrNow.Controllers
{
    
        public class ChatController : Controller
        {
            public IActionResult Index()
            {
                return View();
            }
        }
    }
