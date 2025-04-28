namespace Ecommerce.Application.DTOs;


public class CreateRatingDto
{
  public Guid ProductId { get; set; }
  public int RatingValue { get; set; }  // 1-5
  public string Comment { get; set; }
}
