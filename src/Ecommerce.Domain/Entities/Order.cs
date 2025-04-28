using Ecommerce.Domain.Enums;

namespace Ecommerce.Domain.Entities;
public class Order
{
  public Guid Id { get; set; }
  public string OrderCode { get; set; }
  public string UserId { get; set; }
  public decimal TotalAmount { get; set; }
  public string ShippingAddress { get; set; }
  public DateTime? DeliveryDate { get; set; }
  public string PaymentMethod { get; set; }
  public OrderStatus Status { get; set; }
  public string Note { get; set; }
  public DateTime CreatedDate { get; set; }
  public DateTime UpdatedDate { get; set; }
}
