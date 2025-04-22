

using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.CustomerApp.Controllers;

public class OrderController : Controller
{
  public IActionResult Index()
  {
    return View();
  }

  public IActionResult Detail()
  {
    return View();
  }

  public IActionResult Success()
  {
    return View();
  }

  public IActionResult History()
  {
    return View();
  }

  [HttpPost]
  public IActionResult Submit()
  {
    // ❗ Tạm thời chưa xử lý dữ liệu, chỉ redirect sang thành công
    return RedirectToAction("Success");
  }

}
