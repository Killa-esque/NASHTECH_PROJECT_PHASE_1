namespace Ecommerce.Shared.DTOs;
public class RatingDto
{
  public Guid Id { get; set; }
  public Guid ProductId { get; set; }
  public string UserId { get; set; }
  public string UserName { get; set; }
  public int RatingValue { get; set; }
  public string Comment { get; set; }
  public DateTime CreatedDate { get; set; }
}
