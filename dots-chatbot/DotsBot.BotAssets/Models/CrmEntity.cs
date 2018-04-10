using System.Collections.Generic;

namespace DotsBot.BotAssets.Models
{
    public class CrmEntity
    {
        public Customer Customer { get; set; }

        public Product Product { get; set; }

        public string Salutation { get; set; }
    }
}