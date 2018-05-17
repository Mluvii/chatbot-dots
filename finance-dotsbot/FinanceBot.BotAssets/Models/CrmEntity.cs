namespace FinanceBot.BotAssets.Models
{
    public class CrmEntity
    {
        public Customer Customer { get; set; }

        public InsuranceOffer InsuranceOffer { get; set; }
        public Product Product { get; set; }
        public InsuranceSelection InsuranceSelection { get; set; }

        public string Salutation { get; set; }
    }
}
