namespace FinanceBot.BotAssets.Models
{
    public class Product
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductPhotoUrl { get; set; }
        public decimal? ProductPrice { get; set; }
        public double? InterestRate { get; set; }
    }
}