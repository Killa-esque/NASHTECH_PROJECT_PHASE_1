namespace Ecommerce.Shared.ViewModels;
public class OrderViewModel
{
  public Guid Id { get; set; }
  public DateTime CreatedDate { get; set; }
  public List<OrderItemViewModel> Items { get; set; }
  public decimal TotalAmount { get; set; }
}
