using System.Collections.Generic;
using DotsBot.BotAssets.Models;
using iCord.OnifWebLib.Linq;

namespace DotsBot.BLL
{
    public interface ICrmService
    {
        CrmEntity GetCrmData(string personId);
    }

    public class FakeCrmService : ICrmService
    {
        public static Dictionary<string, CrmEntity> repo = new Dictionary<string, CrmEntity>
        {
            {
                "fe384699-4802-4768-ab0c-829e80aa4e5e", new CrmEntity
                {
                    FirstName = "Petr",
                    LastName = "Dobroucky",
                    Order = new Order
                    {
                        ProductName = "Mydlo",
                        ProductPhotoUrl = "https://images-na.ssl-images-amazon.com/images/I/61onlmYlhxL._SX466_.jpg",
                        ProductPrice = 10000
                    }
                }
            },
            {
                "c82c8fe2-69c2-4ecf-9e3b-ae0e6d963617", new CrmEntity
                {
                    FirstName = "Yury",
                    LastName = "Nudga",
                    Order = new Order
                    {
                        ProductName = "Mydlo",
                        ProductPhotoUrl = "https://images-na.ssl-images-amazon.com/images/I/61onlmYlhxL._SX466_.jpg",
                        ProductPrice = new decimal(7.50)
                    }
                }
            },
            {
                "00000000-0000-0000-0000-000000000001", new CrmEntity
                {
                    FirstName = "Jan",
                    LastName = "Novak",
                    Order = new Order
                    {
                        ProductName = "TheWatch",
                        ProductPhotoUrl = "http://cdn.watchshop.com/profiler/thumb_cache/zoom/99959546_v_1423870690.jpg",
                        ProductPrice = new decimal(100.50)
                    }
                }
            }
        };

        public CrmEntity GetCrmData(string personId)
        {
            return repo.ValueOrDefault(personId,
                new CrmEntity
                {
                    FirstName = "Pan",
                    LastName = "Neznamy",
                    Order = new Order
                    {
                        ProductName = "Nic"
                    }
                });
        }
    }
}