using System;
using System.Collections.Generic;

namespace WebApplication1.Models.Db;

public partial class Payment
{
    public int PaymentId { get; set; }
    public int? OrderId { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal? Amount { get; set; }
    public string? ReceiptImageUrl { get; set; }
    public DateTime? Paymentdate { get; set; }
    public int? PaymentStatus { get; set; }
    public Order? Order { get; set; }
}
