namespace Ecommerce.Shared.Requests;
public class CartRequest
{
  public Guid ProductId { get; set; }
  public int Quantity { get; set; }
}
