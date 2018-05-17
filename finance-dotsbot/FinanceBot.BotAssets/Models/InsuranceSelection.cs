namespace FinanceBot.BotAssets.Models
{
    public class InsuranceSelection
    {
        public BillingPeriods SelectedBillingPlan { get; set; }
        public PlanTypes SelectedCoveragePlan { get; set; }
        public decimal FinalPrice { get; set; }
    }
    
    
}
