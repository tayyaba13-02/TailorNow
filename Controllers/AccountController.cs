using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    public IActionResult Register()
    {
        return Redirect("/Identity/Account/Register");
    }

    public IActionResult Login()
    {
        return Redirect("/Identity/Account/Login");
    }

    public IActionResult Logout()
    {
        return Redirect("/Identity/Account/Logout"); 
    }

    public ViewResult ForgotPassword()
    {
        return View();
    }

    public ViewResult ResetPassword()
    {
        return View();
    }
}
