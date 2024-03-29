﻿namespace FoodShop.Api.Order.Model;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public string? Description { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public Guid? DeliveryInfoId { get; set; }
    public DeliveryInfo? DeliveryInfo { get; set; }
    public ICollection<OrderCalculation> OrderCalculations { get; set; } = new List<OrderCalculation>();
}
