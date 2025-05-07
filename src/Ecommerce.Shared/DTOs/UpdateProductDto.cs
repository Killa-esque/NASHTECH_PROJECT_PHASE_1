
namespace Ecommerce.Shared.DTOs;

public class UpdateProductDto
{
  public string Name { get; set; }
  public string Description { get; set; }
  public decimal Price { get; set; }
  public Guid CategoryId { get; set; }
  public int Stock { get; set; }
  public List<string> ImageUrls { get; set; } = new List<string>();
  public string Weight { get; set; } // Ví dụ: "500g"
  public string Ingredients { get; set; } // Ví dụ: "Bột mì, trứng gà, sữa, đường, kem tươi"
  public string ExpirationDate { get; set; } // Ví dụ: "3 ngày kể từ ngày sản xuất"
  public string StorageInstructions { get; set; } // Ví dụ: "Để mát (2-6°C)"
  public string Allergens { get; set; } // Ví dụ: "Chứa gluten và sữa"
}

