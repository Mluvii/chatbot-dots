using System.Collections.Generic;
using System.Threading.Tasks;
using DotsBot.BotAssets.Models;
using iCord.OnifWebLib.Linq;

namespace DotsBot.BLL
{
    public class FakeCrmService : ICrmService
    {
        public static CrmEntity DefaultEntity = new CrmEntity
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
            Salutation = "Ahoj Honzo!"
        };
        
        public static Dictionary<string, CrmEntity> repo = new Dictionary<string, CrmEntity>
        {
            {
                "fe384699-4802-4768-ab0c-829e80aa4e5e", new CrmEntity
                {
                    Customer = new Customer
                    {
                        FirstName = "Petr",
                        LastName = "Dobroucky",
                    },
                    Product = new Product
                    {
                        ProductName = "Mydlo",
                        ProductPhotoUrl = "https://images-na.ssl-images-amazon.com/images/I/61onlmYlhxL._SX466_.jpg",
                        ProductPrice = 10000,
                        InterestRate = 1.0,
                    },
                    Salutation = "Ahoj Petre!",
                    
                }
            },
            {
                "c82c8fe2-69c2-4ecf-9e3b-ae0e6d963617", new CrmEntity
                {
                    Customer = new Customer
                    {
                        FirstName = "Yury",
                        LastName = "Nudga",
                    },
                    Product = new Product
                    {
                        ProductName = "Mydlo",
                        ProductPhotoUrl = "https://images-na.ssl-images-amazon.com/images/I/61onlmYlhxL._SX466_.jpg",
                        ProductPrice = new decimal(7.50),
                        InterestRate = 0.97,
                    },
                    Salutation = "Ahoj Yury!"
                }
            },
            {
                Customer.DefaultPersonId, DefaultEntity
            }
        };

        public Task<CrmEntity> GetCrmData(string personId)
        {
            return Task.FromResult(repo.ValueOrDefault(personId, DefaultEntity));
        }
    }
}