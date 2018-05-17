using System.Text.RegularExpressions;

namespace FinanceBot.BotAssets
{
    public static class RegexConstants
    {
        public const string Email =
            @"[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

        public const string Phone = @"^\d{9}$";
        public static readonly Regex Expiration = new Regex(@"^\d{2}/\d{2}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex Cvv = new Regex(@"^\d{3}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex CardNumber = new Regex(@"\d{13,}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex CONFUSED =
            new Regex(@"^.*(pomoc|nevim|nevím|help|co dál|co teď|co můžu říct).*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
