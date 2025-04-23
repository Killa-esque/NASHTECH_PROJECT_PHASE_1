namespace Ecommerce.Domain.Entities;
public class Order
{
  public Guid Id { get; set; }
  public string OrderCode { get; set; }
  public Guid UserId { get; set; } // FK đến AspNetUsers
  public decimal TotalAmount { get; set; }
  public string ShippingAddress { get; set; }
  public DateTime? DeliveryDate { get; set; }
  public string PaymentMethod { get; set; }
  public string Status { get; set; }
  public string Note { get; set; }
  public DateTime CreatedDate { get; set; }
}
