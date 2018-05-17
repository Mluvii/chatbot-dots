using System.Globalization;
using System.Threading.Tasks;
using FinanceBot.BotAssets.Models;

namespace FinanceBot.BLL
{
    public interface ICrmService
    {
        Task<CrmEntity> GetCrmData(string personId, CultureInfo culture);
    }
}
