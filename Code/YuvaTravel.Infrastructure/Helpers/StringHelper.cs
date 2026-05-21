using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using YuvaTravel.Base.Constants;
using YuvaTravel.Infrastructure.Dto;

namespace YuvaTravel.Infrastructure.Helpers
{
    public class StringHelper
    {
        /// <summary>
        /// 取得首頁網址
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetHost(Uri uri)
        {
            string str;

            if (uri.PathAndQuery == "/")
            {
                str = uri.AbsoluteUri.TrimEnd('/');
            }
            else
            {
                str = uri.AbsoluteUri.Replace(uri.PathAndQuery, "");
            }

            return str;
        }

        /// <summary>
        /// 取得推廣碼
        /// </summary>
        /// <param name="absolutePathToLower"></param>
        /// <returns></returns>
        public static string GetPromotionCode(string absolutePathToLower)
        {
            string value = "";

            if (absolutePathToLower.Contains("/category") || absolutePathToLower.Contains($"/{nameof(StrPath.Agent).ToLower()}")) // 文章類別頁、招募代理頁
            {
                value = "seobet0101";
            }
            else if (absolutePathToLower.Contains($"/{nameof(StrPath.Search).ToLower()}")) // 搜尋頁
            {
                value = "seobet0102";
            }
            else if (absolutePathToLower.Contains("/tag")) // 標籤頁
            {
                value = "seobet0103";
            }
            else if (absolutePathToLower.Contains($"/{nameof(StrPath.NotFound).ToLower()}")) // 找不到頁面
            {
                value = "seobet0160";
            }
            else if (absolutePathToLower.Contains($"/{nameof(StrPath.Error).ToLower()}")) // 錯誤頁
            {
                value = "seobet0161";
            }
            else // 首頁(預設)
            {
                value = "seobet0100";
            }

            return value;
        }

        /// <summary>
        /// 取得使用者裝置資訊
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string GetUserPlatform(string userAgent)
        {
            string value = "";

            var ua = userAgent;

            if (ua != null)
            {
                if (ua.Contains("Android"))
                    return ua;
                //return $"Android {GetMobileVersion(ua, "Android")}";

                if (ua.Contains("iPad"))
                    return ua;
                //return $"iPad OS {GetMobileVersion(ua, "OS")}";

                if (ua.Contains("iPhone"))
                    return ua;
                //return $"iPhone OS {GetMobileVersion(ua, "OS")}";

                if (ua.Contains("Windows Phone"))
                    return ua;
                //return $"Windows Phone {GetMobileVersion(ua, "Windows Phone")}";

                if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
                    return "Kindle Fire";

                if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
                    return "Black Berry";

                if (ua.Contains("Mac OS"))
                    return "Mac OS";

                if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
                    return "Windows XP";

                if (ua.Contains("Windows NT 6.0"))
                    return "Windows Vista";

                if (ua.Contains("Windows NT 6.1"))
                    return "Windows 7";

                if (ua.Contains("Windows NT 6.2"))
                    return "Windows 8";

                if (ua.Contains("Windows NT 6.3"))
                    return "Windows 8.1";

                if (ua.Contains("Windows NT 10"))
                    return "Windows 10";

                //fallback to basic platform:
                value += (ua.Contains("Mobile") ? " Mobile " : "");
            }
            return value;
        }

        /// <summary>
        /// 取得瀏覽器資訊
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static string GetBrowserInfo(HttpRequest Request)
        {
            StringBuilder sb = new StringBuilder();
            if (Request != null)
            {
                string userAgent = Request.Headers["User-Agent"].ToString();
                sb.AppendLine("----------Browser----------");
                sb.AppendLine($"IP：{Request.HttpContext.Connection.RemoteIpAddress}");
                sb.AppendLine($"SourceUrl：{Request.Headers["Referer"].ToString()}");
                sb.AppendLine($"DestinationUrl：{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                sb.AppendLine($"IsMobile：(需改用 UAParser 等套件偵測)");
                sb.AppendLine($"OS：{StringHelper.GetUserPlatform(userAgent)}");
                sb.AppendLine($"UserAgent：{userAgent}");
                sb.AppendLine($"BrowserPlatform：(需改用 UAParser 等套件偵測)");
                sb.AppendLine($"BrowserType：(需改用 UAParser 等套件偵測)");
                sb.AppendLine($"BrowserVersion：(需改用 UAParser 等套件偵測)");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 取得手機裝置版本
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        private string GetMobileVersion(string userAgent, string deviceName)
        {
            var startIndex = userAgent.IndexOf(deviceName);
            var endIndex = userAgent.IndexOf(';', startIndex);
            var str = userAgent.Substring(startIndex, endIndex - startIndex).Replace(deviceName, "");

            return str;
        }

        /// <summary>
        /// 拆解 DateTimeRange
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static DateTimeRangeDto GetDateTimeRangeData(string range, bool isDate = true)
        {
            var dto = new DateTimeRangeDto();

            if (!string.IsNullOrWhiteSpace(range))
            {
                var temp = range.Split('-');
                if (temp.Length > 0 && DateTime.TryParse(temp[0], out DateTime dt))
                {
                    dto.StartTime = dt;
                }
                if (temp.Length > 1 && DateTime.TryParse(temp[1], out dt))
                {
                    dto.EndTime = dt;
                    if (isDate)
                    {
                        dto.EndTime = dto.EndTime.Value.AddDays(1).AddSeconds(-1);
                    }
                }
            }

            return dto;
        }

    }
}
