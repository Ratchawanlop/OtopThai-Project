using System;

namespace WebApplication1.Models.Db
{
    public class ProductReview
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public string ReviewerName { get; set; }
        public int StarScore { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public virtual Product Product { get; set; } = null!;
        public int? OrderId { get; set; }
        public int? UserId { get; set; }
    }
}