namespace Ecommerce.API.Requests;
public class AddToCartRequest
{
  public Guid ProductId { get; set; }
  public int Quantity { get; set; }
}
