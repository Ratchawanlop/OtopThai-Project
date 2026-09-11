namespace WebApplication1.Models.Db
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public decimal? RatingScore { get; set; }
        public int? ReviewCount { get; set; }
    }
}