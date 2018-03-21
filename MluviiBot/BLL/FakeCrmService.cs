using MluviiBot.BotAssets.Models;

namespace MluviiBot.BLL
{
    public interface ICrmService
    {
        CrmEntity GetCrmData(string personId);
    }

    public class FakeCrmService : ICrmService
    {
        public CrmEntity GetCrmData(string personId)
        {
            return new CrmEntity
            {
                FirstName = "Jan",
                LastName = "Novak",
                Order = new Order
                {
                    ProductName = "XXX",
                    ProductPhotoUrl = "http://cdn.watchshop.com/profiler/thumb_cache/zoom/99959546_v_1423870690.jpg"
                }
            };
        }
    }
}