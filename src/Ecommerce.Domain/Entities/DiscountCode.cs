namespace Ecommerce.Domain.Entities;
public class DiscountCode
{
  public Guid Id { get; set; }
  public string Code { get; set; }
  public decimal DiscountValue { get; set; }
  public string DiscountType { get; set; }
  public DateTime? ExpiryDate { get; set; }
  public bool IsActive { get; set; }
}
