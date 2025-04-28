namespace Ecommerce.Shared.ViewModels;

public class ProductViewModel
{
  public Guid Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public decimal Price { get; set; }
  public string ImageUrl { get; set; }
  public string CategoryName { get; set; } // Display category name
}
