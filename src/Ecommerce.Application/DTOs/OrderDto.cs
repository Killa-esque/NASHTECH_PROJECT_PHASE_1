namespace Ecommerce.Application.DTOs;

public class OrderDto
{
  public Guid Id { get; set; }
  public string UserId { get; set; }
  public string OrderCode { get; set; }
  public string ShippingAddress { get; set; }
  public string PaymentMethod { get; set; }
  public string Note { get; set; }
  public string Status { get; set; }
  public DateTime CreatedDate { get; set; }
  public List<OrderItemDto> Items { get; set; }
  public decimal TotalAmount { get; set; }
}
