using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuvaTravel.Base.Constants
{
    public static class StrText
    {
        #region CRUD

        public const string Dashboard = "Dashboard";

        public const string Index = "列表";
        public const string ReturnIndex = "回列表";
        public const string Create = "新增";
        public const string Edit = "修改";
        public const string Delete = "刪除";
        public const string Details = "檢視";
        public const string Contacts = "聯繫";
        public const string Reviews = "審核";
        public const string Import = "匯入";
        public const string Export = "匯出";
        public const string Save = "儲存";
        public const string ConfirmDelete = "確認刪除";
        public const string Reply = "回覆";
        public const string Generally = "一般";
        public const string Image = "圖片";
        public const string Meta = "Meta";
        public const string Permissions = "權限";
        public const string Upload = "上傳";

        public const string ArticleInfo = "文章資訊";
        public const string Tags = "標籤";
        public const string ArticleCategories = "文章類別";
        public const string Articles = "文章";
        public const string ArticleReviews = "文章審核";
        public const string ArticleComments = "文章留言";
        public const string ArticleSDCommonQuestion = "結構化資料-常見問題";

        public const string WebPageInfo = "網頁資訊";
        public const string WebConfigs = "網頁設定";
        public const string WebMetas = "網頁Meta";
        public const string WebBanners = "網頁Banner";
        public const string UrlReferrers = "網頁來源";
        public const string UrlReferrerCodes = "網頁來源代碼";
        public const string HotKeywords = "熱門關鍵字";
        public const string IpBlockings = "IP黑名單";
        public const string IpOpenings = "IP白名單";
        public const string QuestionAnswers = "常見問答Q&A";

        public const string FuncGroups = "功能群組";
        public const string FuncPrograms = "功能程式";
        public const string UserInfo = "使用者資訊";
        public const string Users = "使用者";
        public const string UserGroups = "使用者群組";
        public const string UserForgetPasswords = "使用者忘記密碼";
        public const string Profiles = "個人資料";
        public const string Information = "基本資料";

        public const string FormInfo = "表單資訊";
        public const string RecruitAgents = "招募代理";

        public const string Guides = "嚮導";
        public const string GuideRoutes = "嚮導路線";
        public const string GuideReviews = "嚮導評論";
        public const string GuideFaqs = "嚮導 FAQ";
        public const string Authors = "作者";
        public const string Regions = "地區";
        public const string Subscribers = "電子報訂閱";

        public const string SignIn = "登入";
        public const string SignOut = "登出";

        public const string ChangePassword = "變更密碼";
        public const string ResetPassword = "重置密碼";

        public const string Mr = "先生";
        public const string Mrs = "女士";

        public const string UploadFileEx = "上傳範例檔";

        #endregion

        #region Filter

        public const string ASC = "遞增排序"; // 由小到大、升序
        public const string DESC = "遞減排序"; // 由大到小、降序

        #endregion

        #region Message

        public const string Success = "成功";
        public const string CreateSuccess = "新增成功";
        public const string EditSuccess = "修改成功";
        public const string DeleteSuccess = "刪除成功";
        public const string ReplySuccess = "回覆成功";
        public const string UploadSuccess = "上傳成功";
        public const string Fail = "失敗";
        public const string Required = "{0}必須輸入";
        public const string RequiredNotName = "必須輸入";
        public const string RequireSelectd = "{0}必須選取";
        public const string OverLength = "{0}不能超過{1}字";
        public const string CodeNotRepeat = "代碼不能重複";
        public const string TitleNotRepeat = "標題不能重複";
        public const string NameNotRepeat = "名稱不能重複";
        public const string UserIdNotRepeat = "帳號不能重複";
        public const string EmailNotRepeat = "Email不能重複";
        public const string IdNotRepeat = "代號不能重複";
        public const string UrlNotRepeat = "Url不能重複";
        public const string IpNotRepeat = "IP不能重複";
        public const string FileSelected = "必須選擇檔案";
        public const string OnlyExcel = "只能上傳Excel";
        public const string NoInputDoubleQuotes = "不能輸入雙引號";

        #endregion

        #region UserWeb

        public const string AccessDenied = "Access Denied";

        public const string ArticleCategory = "文章類別";
        public const string Article = "文章";
        public const string Search = "關鍵字搜尋";
        public const string Tag = "標籤";
        public const string Minute = "分鐘";

        public const string SuperSport = "SUPER體育";
        public const string SuperBall = "SUPER彩球";
        public const string SuperLottery = "SUPER彩票";

        public const string Fairrain = "飛銳";

        public const string XinBao = "鑫寶";

        public const string OG = "OG";

        public const string WM = "WM";

        public const string DG = "DG";

        public const string AllBet = "歐博";

        public const string SA = "SA沙龍";

        public const string Yabo = "亞博";

        public const string ZGChess = "ZG棋牌";
        public const string ZGSlotMachine = "ZG老虎機";
        public const string ZGFishingMachine = "ZG捕魚機";

        public const string AVIA = "AVIA";

        public const string DaliLottery = "大立彩票";
        public const string DaliBall = "大立彩球";

        public const string KingLottery = "王者彩票";
        public const string KingBall = "王者彩球";

        public const string MSGamingFishingMachine = "MSGaming捕魚機";
        public const string MSGamingSlotMachine = "MSGaming老虎機";

        public const string RSGFishingMachine = "RSG皇家捕魚機";
        public const string RSGSlotMachine = "RSG皇家老虎機";

        public const string BNG = "BNG";

        public const string BWINSlotMachine = "BWIN老虎機";
        public const string BWINFishingMachine = "BWIN捕魚機";

        #endregion

        public const string AdminUserId = "System";
        public const string AdminName = "域見後台";
        public const string UserWebName = "域見";
        public const string WebSiteUrl = "http://www.yuvatravel.net/";

        public const string HomePage = "首頁";
        public const string OnlineCustomer = "專人客服";

        public static string SportsLottery = "";
        public static string Baccarat = "";
        public static string SlotMachine = "";
        public static string ColoredBalls = "";
        public static string Lottery = "";
        public static string Chess = "";
        public static string FishingMachine = "";


        public const string XsrfKey = "XsrfId"; // 新增外部登入時用來當做 XSRF 保護
        public const string Login = "登入";
        public const string LinkExternalLogin = "連接登入";
        public const string RemoveExternalLogin = "移除登入";
        public const string LineQrCode = "https://lin.ee/rQgNQ6D";
    }
}
