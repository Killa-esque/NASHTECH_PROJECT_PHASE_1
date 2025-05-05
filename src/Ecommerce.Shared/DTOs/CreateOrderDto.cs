namespace Ecommerce.Shared.DTOs;

public class CreateOrderDto
{
  public string ShippingAddress { get; set; }
  public string PaymentMethod { get; set; }
  public string Note { get; set; }
  public decimal TotalAmount { get; set; }
  public List<OrderItemDto> Items { get; set; }
}



