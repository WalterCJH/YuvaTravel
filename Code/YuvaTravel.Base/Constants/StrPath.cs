using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Base.Constants
{
    public static class StrPath
    {
        #region UploadExPath

        public const string UrlReferrerCodeImportFileFullNameEx = "/UploadEx/網頁來源代碼匯入範例檔.xlsx";

        #endregion

        #region FolderPath

        public const string ImageUserFiles = "Images/UserFiles";
        public const string ImageUserVideos = "Images/UserVideos";

        #endregion

        #region ImagesPath

        public const string Logo = "/Images/Logo.png";

        #endregion

        #region UrlPath

        /// <summary>
        /// 「開始媒合」按鈕指向的外部連結。日後改 Line 官方帳號或其他通路時,改這一處即可。
        /// </summary>
        public const string MatchStart = "https://www.facebook.com/YuvaTravel";

        /// <summary>
        /// Facebook 粉絲頁。
        /// </summary>
        public const string Facebook = "https://www.facebook.com/YuvaTravel";

        /// <summary>
        /// Instagram 帳號。尚未啟用前以 # 佔位,避免前端 404。
        /// </summary>
        public const string Instagram = "#";

        public const string Click = "/Click?u=";

        public static string Category(string host, string code)
        {
            var data = $"{host}/Category";
            if (!string.IsNullOrEmpty(code))
                data = $"{data}/{code}";
            return data;
        }

        public static string ArticleCategory(string host, string id)
        {
            return Category(host, id);
        }

        public static string Region(string host, string code)
        {
            var data = $"{host}/Region";
            if (!string.IsNullOrEmpty(code))
                data = $"{data}/{code}";
            return data;
        }

        public static string GuideProfile(string host, string id)
        {
            var data = $"{host}/GuideProfile";
            if (!string.IsNullOrEmpty(id))
                data = $"{data}/{id}";
            return data;
        }

        public static string Agent(string host, string id)
        {
            var data = $"{host}/Agent";
            if (!string.IsNullOrEmpty(id))
                data = $"{data}/{id}";
            return data;
        }

        public static string Article(string host, string id)
        {
            var data = $"{host}/Article";
            if (!string.IsNullOrEmpty(id))
            {
                data = $"{data}/{id}";
            }
            return data;
        }

        public static string Search(string keyword)
        {
            var data = $"/Search";
            if (!string.IsNullOrEmpty(keyword))
            {
                data = $"{data}?keyword={keyword}";
            }
            return data;
        }

        public static string Tag(string name)
        {
            var data = $"/Tag";
            if (!string.IsNullOrEmpty(name))
            {
                data = $"{data}/{Uri.EscapeDataString(name)}";
            }
            return data;
        }

        public static string NotFound(string id)
        {
            var data = $"/NotFound";
            if (!string.IsNullOrEmpty(id))
            {
                data = $"{data}/{id}";
            }
            return data;
        }

        public static string Error(string id)
        {
            var data = $"/Error";
            if (!string.IsNullOrEmpty(id))
            {
                data = $"{data}/{id}";
            }
            return data;
        }

        public static string ClickUrl(string host, string url, string m = null)
        {
            var data = $"{host}{Click}{url}";
            if (!string.IsNullOrEmpty(m))
            {
                data = $"{data}&m={m}";
            }
            return data;
        }

        #endregion
    }
}
