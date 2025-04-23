namespace Ecommerce.Shared.ViewModels;
public class RatingViewModel
{
  public Guid Id { get; set; }
  public string UserName { get; set; }
  public int RatingValue { get; set; }
  public string Comment { get; set; }
  public DateTime CreatedDate { get; set; }
}
