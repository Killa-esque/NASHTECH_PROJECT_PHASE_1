using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Ecommerce.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
  // Đây là endpoint để PayOS hoặc Momo gọi về khi thanh toán thành công
  [HttpPost("payment-success")]
  public IActionResult PaymentSuccess([FromBody] PaymentNotification payload)
  {
    // 👉 1. Xác minh payload (nếu có signature/checksum từ PayOS/Momo)
    if (!VerifyChecksum(payload)) return BadRequest();

    // 👉 2. Xử lý cập nhật trạng thái đơn hàng
    // VD: cập nhật đơn hàng có mã = payload.OrderId thành "Đã thanh toán"
    Console.WriteLine("Đã nhận webhook thanh toán thành công cho đơn hàng: " + payload.OrderId);

    // 👉 3. Lưu log / gửi email / xử lý hậu kỳ

    return Ok();
  }

  private bool VerifyChecksum(PaymentNotification payload)
  {
    // TODO: tuỳ từng bên PayOS/Momo mà có thuật toán verify khác nhau
    // Có thể dựa vào payload.Signature và SecretKey
    return true;
  }
}

public class PaymentNotification
{
  public string OrderId { get; set; } = string.Empty;
  public string TransactionId { get; set; } = string.Empty;
  public decimal Amount { get; set; }
  public string Status { get; set; } = string.Empty; // success / failed
  public string Signature { get; set; } = string.Empty; // để xác minh (tuỳ bên cung cấp)
}
