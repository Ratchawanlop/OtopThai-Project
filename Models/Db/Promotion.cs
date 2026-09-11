using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Db
{
    public class Promotion
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public string PromotionType { get; set; } 
        public decimal DiscountValue { get; set; } 
        public bool IsPercentage { get; set; } 
        public decimal MinPurchaseAmount { get; set; } 
        public string? TargetCategoryName { get; set; }
        public string? BundleCategoryName { get; set; } 
        public bool IsActive { get; set; } = true;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}