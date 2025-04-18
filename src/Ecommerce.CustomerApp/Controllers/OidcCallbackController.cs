
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.CustomerApp.Controllers;

[Route("signin-oidc")]
public class OidcCallbackController : Controller
{
    [HttpGet]
    public IActionResult Callback()
    {
        // Bạn không cần xử lý thủ công nếu dùng middleware
        // Chỉ dùng khi muốn override toàn bộ
        return Ok("Đã xử lý callback signin-oidc!");
    }
}

