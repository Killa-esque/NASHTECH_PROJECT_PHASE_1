namespace Ecommerce.Shared.ViewModels;
public class PaymentTransactionViewModel
{
  public Guid Id { get; set; }
  public string Provider { get; set; }
  public string TransactionCode { get; set; }
  public decimal Amount { get; set; }
  public string Status { get; set; }
}
