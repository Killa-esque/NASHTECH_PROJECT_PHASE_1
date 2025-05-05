using System;

namespace Ecommerce.Shared.ViewModels;

public class OrderViewModel
{
  public Guid Id { get; set; }
  public string OrderCode { get; set; }
  public string ShippingAddress { get; set; }
  public string PaymentMethod { get; set; }
  public string Note { get; set; }
  public string Status { get; set; }
  public DateTime CreatedDate { get; set; }
  public DateTime? DeliveryDate { get; set; }
  public decimal TotalAmount { get; set; }
  public List<OrderItemViewModel> Items { get; set; }
}

public class CreateOrderViewModel
{
  public string ShippingAddress { get; set; }
  public string PaymentMethod { get; set; }
  public string Note { get; set; }
  public decimal TotalAmount { get; set; }
  public List<OrderItemViewModel> Items { get; set; }
}
