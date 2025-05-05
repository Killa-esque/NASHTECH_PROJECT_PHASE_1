namespace Ecommerce.Shared.DTOs;
public class PaymentTransactionDto
{
  public Guid Id { get; set; }
  public Guid OrderId { get; set; }
  public string Provider { get; set; }
  public string TransactionCode { get; set; }
  public decimal Amount { get; set; }
  public string Status { get; set; }
}
