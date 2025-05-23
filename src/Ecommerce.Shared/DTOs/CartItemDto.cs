namespace Ecommerce.Shared.DTOs;
public class CartItemDto
{
  public Guid Id { get; set; }
  public Guid ProductId { get; set; }
  public string ProductName { get; set; }
  public decimal Price { get; set; }
  public int Quantity { get; set; }
  public decimal Total => Price * Quantity;
  public string ImageUrl { get; set; }
}
