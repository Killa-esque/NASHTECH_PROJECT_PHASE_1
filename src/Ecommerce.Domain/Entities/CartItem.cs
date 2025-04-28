namespace Ecommerce.Domain.Entities;

public class CartItem
{
  public Guid Id { get; set; }
  public string UserId { get; set; } // FK to AspNetUsers
  public Guid ProductId { get; set; }
  public int Quantity { get; set; }
  public DateTime AddedDate { get; set; }
}
