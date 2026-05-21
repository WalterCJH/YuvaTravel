using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace YuvaTravel.Infrastructure.Storage
{
    /// <summary>
    /// 本機 fallback:R2 未啟用時,把圖片轉 WebP 後寫到 wwwroot/Images/...
    /// 回傳相對路徑 /Images/...,前台 view 既有的 StartsWith("http") 判斷會走相對路徑分支
    /// </summary>
    public class LocalImageUploadService : IImageUploadService
    {
        private readonly IWebHostEnvironment _env;
        private const int DefaultQuality = 80;

        public LocalImageUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadAsync(string base64, int maxWidth, string[] folderSegments, string fileNameNoExt)
        {
            var rawBytes = ImageProcessor.DecodeBase64(base64);
            return await Task.FromResult(SaveLocal(rawBytes, maxWidth, folderSegments, fileNameNoExt));
        }

        public async Task<string> UploadAsync(IFormFile file, int maxWidth, string[] folderSegments, string fileNameNoExt)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("檔案為空", nameof(file));

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return SaveLocal(ms.ToArray(), maxWidth, folderSegments, fileNameNoExt);
        }

        private string SaveLocal(byte[] rawBytes, int maxWidth, string[] folderSegments, string fileNameNoExt)
        {
            var webpBytes = ImageProcessor.ToWebp(rawBytes, maxWidth, DefaultQuality);
            var fileName = ImageProcessor.NormalizeWebpFileName(fileNameNoExt);

            var folders = (folderSegments ?? Array.Empty<string>())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim('/'))
                .ToArray();

            var relativeFolder = "Images/" + string.Join('/', folders);
            var absoluteFolder = Path.Combine(_env.WebRootPath, relativeFolder.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(absoluteFolder);

            var absolutePath = Path.Combine(absoluteFolder, fileName);
            File.WriteAllBytes(absolutePath, webpBytes);

            return "/" + relativeFolder + "/" + fileName;
        }

        public Task DeleteAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("/Images/", StringComparison.OrdinalIgnoreCase))
                return Task.CompletedTask;

            try
            {
                var relative = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var absolutePath = Path.Combine(_env.WebRootPath, relative);
                if (File.Exists(absolutePath)) File.Delete(absolutePath);
            }
            catch { /* 忽略刪檔失敗 */ }

            return Task.CompletedTask;
        }
    }
}
