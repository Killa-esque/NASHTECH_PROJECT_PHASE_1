namespace Ecommerce.Application.DTOs;

public class OrderDto
{
  public Guid Id { get; set; }
  public DateTime CreatedDate { get; set; }
  public List<OrderItemDto> Items { get; set; }
  public decimal TotalAmount { get; set; }
}
