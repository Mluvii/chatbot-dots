using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using DotsBot.BotAssets.Models;
using iCord.OnifWebLib.Linq;

namespace DotsBot.BLL
{
    public class FakeCrmService : ICrmService
    {
        private CultureInfo cultureCz = new CultureInfo("cs");
        
        public static CrmEntity DefaultEntityEN = new CrmEntity
        {
            Customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
            },
            Product = new Product
            {
                ProductName = "TheWatch",
                ProductPhotoUrl = "http://cdn.watchshop.com/profiler/thumb_cache/zoom/99959546_v_1423870690.jpg",
                ProductPrice = new decimal(100.50),
                InterestRate = 0.88,
            },
            Salutation = "Hi John"
        };
        
        public static CrmEntity DefaultEntityCZ = new CrmEntity
        {
            Customer = new Customer
            {
                FirstName = "Jan",
                LastName = "Novak",
            },
            Product = new Product
            {
                ProductName = "TheWatch",
                ProductPhotoUrl = "http://cdn.watchshop.com/profiler/thumb_cache/zoom/99959546_v_1423870690.jpg",
                ProductPrice = new decimal(100.50),
                InterestRate = 0.88,
            },
            Salutation = "Ahoj Honzo"
        };

        public Task<CrmEntity> GetCrmData(string personId, CultureInfo culture)
        {
            return Task.FromResult(culture.TwoLetterISOLanguageName.Equals(cultureCz.TwoLetterISOLanguageName) ? DefaultEntityCZ : DefaultEntityEN);
        }
    }
}