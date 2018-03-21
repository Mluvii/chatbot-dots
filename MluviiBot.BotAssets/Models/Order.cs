using System;

namespace MluviiBot.BotAssets.Models
{
    public class Order
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductPhotoUrl { get; set; }
        public Decimal ProductPrice { get; set; }
    }
}