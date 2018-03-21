using System;
using System.Globalization;
using System.Linq;

namespace MluviiBot.BotAssets.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsAnyIgnoreCaseAndAccents(this string baseString, params string[] textsToSearch)
        {
            return textsToSearch.Any(t => baseString.ContainsIgnoreCaseAndAccents(baseString));
        }
        
        public static bool ContainsIgnoreCaseAndAccents(this string baseString, string textToSearch)
        {
            CompareInfo ci = new CultureInfo("").CompareInfo;
            CompareOptions co = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;
            
            return ci.IndexOf(baseString, textToSearch, co) != -1;
        }
    }
}