using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Db;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public string? ShippingAddress { get; set; }

    public decimal? Subtotal { get; set; }
    public decimal? ShippingCost { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? NetAmount { get; set; }

    public string? TrackingStatus { get; set; }
    public string? PaymentSlipUrl { get; set; }
    public DateTime? OrderDatetime { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
}