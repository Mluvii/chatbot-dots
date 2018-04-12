using System.Globalization;
using System.Linq;
using System.Text;

namespace DotsBot.BotAssets.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsAnyIgnoreCaseAndAccents(this string baseString, params string[] textsToSearch)
        {
            return textsToSearch.Any(baseString.ContainsIgnoreCaseAndAccents);
        }

        public static bool ContainsIgnoreCaseAndAccents(this string baseString, string textToSearch)
        {
            var ci = new CultureInfo("").CompareInfo;
            var co = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

            return ci.IndexOf(baseString, textToSearch, co) != -1;
        }
        
        public static string RemoveDiacritics(this string text) 
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}