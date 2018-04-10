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

        public async Task<CrmEntity> GetCrmData(string personId)
        {
            if (crmUrl == null)
            {
                return await GetDummyResponse(personId); 
            }
            using (var client = new HttpClient())
            {
                var customerTask = GetCustomerDetail(client, personId);
                var productTask = GetProduct(client, personId);

                await Task.WhenAll(customerTask, productTask);

                var customer = await customerTask;
                var product = await productTask;

                if (customer?.Customer == null || product == null)
                {
                    return await GetDummyResponse(personId);
                }
                
                customer.Product = product;

                return customer;
            }
        }

        private static async Task<CrmEntity> GetDummyResponse(string personId)
        {
            var fakeCrm = new FakeCrmService();

            return await fakeCrm.GetCrmData(personId);
        }

        private async Task<CrmEntity> GetCustomerDetail(HttpClient client, string personId)
        {
            CrmEntity customer = null;
            HttpResponseMessage response = await client.GetAsync(crmUrl + $"/GetCustomerDetailById/{personId}");
            if (response.IsSuccessStatusCode)
            {
                customer = await response.Content.ReadAsAsync<CrmEntity>();
            }
            
            return customer;
        }
        
        private async Task<Product> GetProduct(HttpClient client, string personId) 
        {
            Product product = null;
            HttpResponseMessage response = await client.GetAsync(crmUrl + $"/GetLastPOStransaction/{personId}");
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<Product>();
            }
            
            return product;
        }

    }
}