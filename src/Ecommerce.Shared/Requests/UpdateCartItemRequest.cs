namespace Ecommerce.Shared.Requests;

public class UpdateCartItemRequest
{
  public Guid ProductId { get; set; }
  public int Quantity { get; set; }
}
