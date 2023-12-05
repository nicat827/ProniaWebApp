using Microsoft.EntityFrameworkCore;

namespace Pronia.Utilities.Extensions
{
    public  static class DataHelper
    {
        public static string Capitalize(this string text)
        {
            text = text.Trim();
            return text.Substring(0, 1).ToUpper() + text.Substring(1).ToLower();
        }
    }
}
