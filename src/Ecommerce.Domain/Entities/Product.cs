namespace Ecommerce.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsFeatured { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
