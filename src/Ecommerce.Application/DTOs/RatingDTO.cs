namespace Ecommerce.Application.DTOs;

public class RatingDTO
{
  public Guid Id { get; set; }
  public Guid ProductId { get; set; }
  public Guid UserId { get; set; }
  public int? RatingValue { get; set; }
  public string? Comment { get; set; }
}
