namespace Ecommerce.Domain.Entities;
public class PaymentTransaction
{
  public Guid Id { get; set; }
  public Guid OrderId { get; set; }
  public string Provider { get; set; } // Momo, PayOS
  public string TransactionCode { get; set; }
  public decimal Amount { get; set; }
  public string Status { get; set; } // Success, Pending, Failed
  public DateTime CreatedDate { get; set; }
}
