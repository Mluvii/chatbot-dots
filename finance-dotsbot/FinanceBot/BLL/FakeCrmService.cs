using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FinanceBot.BotAssets.Models;
using iCord.OnifWebLib.Linq;

namespace FinanceBot.BLL
{
    public class FakeCrmService : ICrmService
    {
        private CultureInfo cultureCz = new CultureInfo("cs");
        
        public static CrmEntity DefaultEntityEN = new CrmEntity
        {
            Salutation = "Hi John",
            Customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
            },
            Product = GetDummyProduct(),
            InsuranceOffer = GetDummyInsuranceOffer()
            
        };
        
        public static CrmEntity DefaultEntityCZ = new CrmEntity
        {
            Salutation = "Ahoj Honzo",
            Customer = new Customer
            {
                FirstName = "Jan",
                LastName = "Novak",
            },
            Product = GetDummyProduct(),
            InsuranceOffer = GetDummyInsuranceOffer()
        };

        public static Product GetDummyProduct()
        {
            return new Product
            {
                ProductName = "TheWatch",
                ProductPhotoUrl = "http://cdn.watchshop.com/profiler/thumb_cache/zoom/99959546_v_1423870690.jpg",
                ProductPrice = new decimal(100.50),
                InterestRate = 0.88,
            };
        }

        public static InsuranceOffer GetDummyInsuranceOffer()
        {
            return new InsuranceOffer
            {
                YearlyDiscountRate = 5,
                Plans = new List<InsurancePlan>
                {
                    new InsurancePlan
                    {
                        PlanType = PlanTypes.DeviceOnly,
                        PriceMonthly = 200
                    },
                    new InsurancePlan
                    {
                        PlanType = PlanTypes.DeviceAndDisplay,
                        PriceMonthly = 250
                    },
                }
            };
        }

        public Task<CrmEntity> GetCrmData(string personId, CultureInfo culture)
        {
            return Task.FromResult(culture.TwoLetterISOLanguageName.Equals(cultureCz.TwoLetterISOLanguageName) ? DefaultEntityCZ : DefaultEntityEN);
        }
    }
}
