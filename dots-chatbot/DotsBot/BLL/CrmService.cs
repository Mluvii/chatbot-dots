using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using DotsBot.BotAssets.Models;

namespace DotsBot.BLL
{
    public class CrmService: ICrmService
    {
        private readonly string crmUrl;

        public CrmService(string crmUrl)
        {
            this.crmUrl = crmUrl;
        }

        public async Task<CrmEntity> GetCrmData(string personId, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(crmUrl))
            {
                return await GetDummyResponse(personId, culture); 
            }
            using (var client = new HttpClient())
            {
                var customerTask = GetCustomerDetail(client, personId, culture);
                var productTask = GetProduct(client, personId, culture);

                await Task.WhenAll(customerTask, productTask);

                var customer = await customerTask;
                var product = await productTask;

                if (customer?.Customer == null 
                    || product?.ProductName == null
                    || product.ProductPrice == null)
                {
                    return await GetDummyResponse(personId, culture);
                }
                
                customer.Product = product;

                return customer;
            }
        }

        private static async Task<CrmEntity> GetDummyResponse(string personId, CultureInfo culture)
        {
            var fakeCrm = new FakeCrmService();

            return await fakeCrm.GetCrmData(personId, culture);
        }

        private async Task<CrmEntity> GetCustomerDetail(HttpClient client, string personId, CultureInfo culture)
        {
            CrmEntity customer = null;
            HttpResponseMessage response = await client.GetAsync(crmUrl + $"/GetCustomerDetailById/{personId}?lang={culture.TwoLetterISOLanguageName}");
            if (response.IsSuccessStatusCode)
            {
                customer = await response.Content.ReadAsAsync<CrmEntity>();
            }
            
            return customer;
        }
        
        private async Task<Product> GetProduct(HttpClient client, string personId, CultureInfo culture) 
        {
            Product product = null;
            HttpResponseMessage response = await client.GetAsync(crmUrl + $"/GetLastPOStransaction/{personId}?lang={culture.TwoLetterISOLanguageName}");
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<Product>();
            }
            
            return product;
        }

    }
}