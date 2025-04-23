namespace Ecommerce.Application.DTOs;
public class DiscountCodeDto
{
  public Guid Id { get; set; }
  public string Code { get; set; }
  public decimal DiscountAmount { get; set; }
  public DateTime ExpiryDate { get; set; }
}
