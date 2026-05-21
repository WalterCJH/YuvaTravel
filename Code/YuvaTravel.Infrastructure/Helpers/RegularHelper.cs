using System.Text.RegularExpressions;

namespace YuvaTravel.Infrastructure.Helpers
{
    public class RegularHelper
    {
        public static bool IsEng(string value)
        {
            Regex regex = new Regex("^[a-zA-Z]+$");
            return regex.IsMatch(value);
        }

    }
}
