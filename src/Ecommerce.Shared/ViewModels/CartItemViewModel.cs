namespace Ecommerce.Shared.ViewModels;
public class CartItemViewModel
{
  public Guid Id { get; set; }
  public Guid ProductId { get; set; }
  public string ProductName { get; set; }
  public int Quantity { get; set; }
  public decimal Price { get; set; }
  public decimal Total { get; set; }
  public string ImageUrl { get; set; }
}
