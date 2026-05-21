using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using YuvaTravel.Base.Constants;

namespace YuvaTravel.Infrastructure.Helpers
{
    public class MessageHelper
    {
        public static string LogMessage(Exception ex, string userId, string urlPath, string browserInfo = null, string dataKey = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"----------{StrText.AdminName}----------");
            sb.AppendLine($"UserId：{userId}");
            sb.AppendLine($"UrlPath：{urlPath}");
            sb.AppendLine($"DataKey：{dataKey}");
            sb.AppendLine("----------Message----------");
            sb.AppendLine(ex?.Message);
            sb.AppendLine("----------Exception----------");
            sb.AppendLine(ex?.StackTrace);
            sb.AppendLine("----------InnerException----------");
            sb.AppendLine(ex?.InnerException?.Message);
            sb.AppendLine("----------InnerException Two----------");
            sb.AppendLine(ex?.InnerException?.InnerException?.Message);
            sb.AppendLine("----------InnerException Three----------");
            sb.AppendLine(ex?.InnerException?.InnerException?.InnerException?.Message);

            // EF Core：驗證錯誤改由 DbUpdateException 處理
            if (ex is DbUpdateException dbEx)
            {
                sb.AppendLine("----------DbUpdateException----------");
                sb.AppendLine(dbEx.InnerException?.Message);
            }

            #region Browser

            if (!string.IsNullOrWhiteSpace(browserInfo))
            {
                sb.AppendLine(browserInfo);
            }

            #endregion

            sb.AppendLine();

            return sb.ToString();
        }

    }
}
