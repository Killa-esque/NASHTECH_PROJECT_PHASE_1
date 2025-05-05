namespace Ecommerce.Shared.DTOs;

public class UpdateOrderDto
{
  public Guid OrderId { get; set; }
  public string? Status { get; set; }
  public string? Note { get; set; }
  public string? ShippingAddress { get; set; }
}
