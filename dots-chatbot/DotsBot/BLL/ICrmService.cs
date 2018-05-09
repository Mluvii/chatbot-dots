using System.Globalization;
using System.Threading.Tasks;
using DotsBot.BotAssets.Models;

namespace DotsBot.BLL
{
    public interface ICrmService
    {
        Task<CrmEntity> GetCrmData(string personId, CultureInfo culture);
    }
}