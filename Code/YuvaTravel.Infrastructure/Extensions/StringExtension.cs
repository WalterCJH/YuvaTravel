using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Infrastructure.Extensions
{
    public static class StringExtension
    {
        public static string Right(this string source, int length)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                return source.Substring(length > source.Length ? 0 : source.Length - length);
            }
            else
            {
                return "";
            }
        }

        public static string StrStr(this string hayStack, string needle, bool beforeNeedle = false)
        {
            if (beforeNeedle)
            {
                return hayStack.Substring(0, hayStack.IndexOf(needle));
            }
            else
            {
                return hayStack.Substring(hayStack.IndexOf(needle));
            }
        }
    }
}
