// Controllers/AccountController.cs
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
  public IActionResult Profile()
  {
    return View();
  }
}
