using System.Collections.Generic;
using iCord.OnifWebLib.Linq;
using MluviiBot.BotAssets.Models;

namespace MluviiBot.BLL
{
    public interface ICrmService
    {
        CrmEntity GetCrmData(string personId);
    }

    public class FakeCrmService : ICrmService
    {
        public static Dictionary<string, CrmEntity> repo = new Dictionary<string, CrmEntity>
        {
            {"fe384699-4802-4768-ab0c-829e80aa4e5e", new CrmEntity
            {
                FirstName = "Petr",
                LastName = "Dobroucky",
                Order = new Order
                {
                    ProductName = "Mydlo",
                    ProductPhotoUrl = "https://images-na.ssl-images-amazon.com/images/I/61onlmYlhxL._SX466_.jpg"
                }
            }},
            {"00000000-0000-0000-0000-000000000001", new CrmEntity
            {
                FirstName = "Jan",
                LastName = "Novak",
                Order = new Order
                {
                    ProductName = "TheWatch",
                    ProductPhotoUrl = "http://cdn.watchshop.com/profiler/thumb_cache/zoom/99959546_v_1423870690.jpg"
                }
            }},
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
                        ProductName = "Nic",
                    }
                });
        }
    }
}