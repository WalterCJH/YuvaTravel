using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace YuvaTravel.Infrastructure.Storage
{
    /// <summary>
    /// 圖片上傳服務。所有實作都會:
    ///   1. 把輸入圖片轉成 WebP
    ///   2. 若原圖寬度 > maxWidth (且 maxWidth > 0),等比縮小到 maxWidth
    ///   3. 不放大 (小於 maxWidth 維持原寬)
    ///   4. 回傳可直接存到 DB 的 URL (R2 模式為完整 https URL,本機模式為 /Images/... 相對路徑)
    /// </summary>
    public interface IImageUploadService
    {
        /// <param name="base64">data URL (data:image/png;base64,xxx) 或純 base64</param>
        /// <param name="maxWidth">目標最大寬度;0 表示不縮放</param>
        /// <param name="folderSegments">資料夾路徑,例 ["articles", "1690"]</param>
        /// <param name="fileNameNoExt">檔名 (不含副檔名,系統一律補 .webp)</param>
        Task<string> UploadAsync(string base64, int maxWidth, string[] folderSegments, string fileNameNoExt);

        Task<string> UploadAsync(IFormFile file, int maxWidth, string[] folderSegments, string fileNameNoExt);

        /// <summary>刪除指定 URL 對應的物件 (R2 mode) 或本機檔案 (Local mode)。非本系統存的 URL 會被忽略。</summary>
        Task DeleteAsync(string url);
    }
}
