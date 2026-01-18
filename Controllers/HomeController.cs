using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TailorrNow.Models;

namespace TailorrNow.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult index()
        {
            return View();
        }
    }
}