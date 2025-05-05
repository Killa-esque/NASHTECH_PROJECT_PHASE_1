namespace Ecommerce.Domain.Entities;

public class ProductImage
{
  public Guid Id { get; set; }
  public Guid ProductId { get; set; }
  public string ImageUrl { get; set; }
  public bool IsPrimary { get; set; }
  public DateTime CreatedDate { get; set; }
}
