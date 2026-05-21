using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace YuvaTravel.Infrastructure.Storage
{
    public class R2ImageUploadService : IImageUploadService, IDisposable
    {
        private readonly R2Options _options;
        private readonly ILogger<R2ImageUploadService> _logger;
        private readonly AmazonS3Client _client;

        public R2ImageUploadService(R2Options options, ILogger<R2ImageUploadService> logger)
        {
            _options = options;
            _logger = logger;

            var config = new AmazonS3Config
            {
                ServiceURL = options.Endpoint,
                ForcePathStyle = true,
                AuthenticationRegion = "auto",
            };
            _client = new AmazonS3Client(options.AccessKeyId, options.SecretAccessKey, config);
        }

        public async Task<string> UploadAsync(string base64, int maxWidth, string[] folderSegments, string fileNameNoExt)
        {
            var rawBytes = ImageProcessor.DecodeBase64(base64);
            return await UploadCoreAsync(rawBytes, maxWidth, folderSegments, fileNameNoExt);
        }

        public async Task<string> UploadAsync(IFormFile file, int maxWidth, string[] folderSegments, string fileNameNoExt)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("檔案為空", nameof(file));

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return await UploadCoreAsync(ms.ToArray(), maxWidth, folderSegments, fileNameNoExt);
        }

        private async Task<string> UploadCoreAsync(byte[] rawBytes, int maxWidth, string[] folderSegments, string fileNameNoExt)
        {
            // 轉 WebP + resize
            var webpBytes = ImageProcessor.ToWebp(rawBytes, maxWidth, _options.WebpQuality);
            var fileName = ImageProcessor.NormalizeWebpFileName(fileNameNoExt);
            var key = BuildKey(folderSegments, fileName);

            using var uploadStream = new MemoryStream(webpBytes);
            var req = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                InputStream = uploadStream,
                ContentType = "image/webp",
                DisablePayloadSigning = true,    // R2 必須關掉,否則 SignatureDoesNotMatch
                Headers = { CacheControl = "public, max-age=31536000, immutable" }
            };

            try
            {
                await _client.PutObjectAsync(req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "R2 上傳失敗 key={Key}", key);
                throw;
            }

            return $"{_options.PublicUrl.TrimEnd('/')}/{key}";
        }

        public async Task DeleteAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            var publicUrl = _options.PublicUrl.TrimEnd('/');
            if (!url.StartsWith(publicUrl, StringComparison.OrdinalIgnoreCase))
            {
                // 非 R2 URL (如本機舊圖 /Images/...),交給呼叫端處理或忽略
                return;
            }

            var key = url.Substring(publicUrl.Length).TrimStart('/');
            try
            {
                await _client.DeleteObjectAsync(_options.BucketName, key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "R2 刪除失敗 key={Key} (忽略不阻斷流程)", key);
            }
        }

        private static string BuildKey(string[] folderSegments, string fileName)
        {
            var folders = (folderSegments ?? Array.Empty<string>())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim('/').Replace(' ', '_').ToLowerInvariant());
            var prefix = string.Join('/', folders);
            return string.IsNullOrEmpty(prefix) ? fileName : $"{prefix}/{fileName}";
        }

        public void Dispose() => _client?.Dispose();
    }
}
