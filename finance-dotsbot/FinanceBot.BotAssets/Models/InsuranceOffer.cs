using System.Collections.Generic;

namespace FinanceBot.BotAssets.Models
{
    public class InsuranceOffer
    {
        public decimal YearlyDiscountRate { get; set; }
        public IList<InsurancePlan> Plans { get; set; }
    }

    public class InsurancePlan
    {
        public PlanTypes PlanType { get; set; }
        public decimal PriceMonthly { get; set; }

    }
}
