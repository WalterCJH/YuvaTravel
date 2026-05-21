using System;
using SkiaSharp;

namespace YuvaTravel.Infrastructure.Storage
{
    /// <summary>SkiaSharp 圖片處理:resize (不放大) + WebP 編碼</summary>
    internal static class ImageProcessor
    {
        /// <summary>
        /// 把任意輸入圖片轉成 WebP。若原圖寬 &gt; maxWidth (且 maxWidth &gt; 0) 會等比縮小;否則維持原尺寸。
        /// </summary>
        public static byte[] ToWebp(byte[] inputBytes, int maxWidth, int quality)
        {
            using var inputData = SKData.CreateCopy(inputBytes);
            using var codec = SKCodec.Create(inputData);
            if (codec == null)
                throw new InvalidOperationException("無法解碼圖片 (格式不支援或檔案損毀)");

            using var original = SKBitmap.Decode(codec);
            if (original == null)
                throw new InvalidOperationException("圖片解碼失敗");

            SKBitmap working = original;
            bool needDispose = false;
            try
            {
                if (maxWidth > 0 && original.Width > maxWidth)
                {
                    int newWidth = maxWidth;
                    int newHeight = (int)Math.Round(original.Height * (newWidth / (double)original.Width));
                    var info = new SKImageInfo(newWidth, newHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
                    var resized = new SKBitmap(info);
                    var sampling = new SKSamplingOptions(SKCubicResampler.Mitchell);
                    if (!original.ScalePixels(resized, sampling))
                    {
                        resized.Dispose();
                        throw new InvalidOperationException("圖片縮放失敗");
                    }
                    working = resized;
                    needDispose = true;
                }

                using var image = SKImage.FromBitmap(working);
                using var encoded = image.Encode(SKEncodedImageFormat.Webp, quality);
                return encoded.ToArray();
            }
            finally
            {
                if (needDispose) working.Dispose();
            }
        }

        /// <summary>把 data URL 或純 base64 解成 byte[]</summary>
        public static byte[] DecodeBase64(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                throw new ArgumentException("base64 為空", nameof(base64));

            var raw = base64;
            int commaIdx = raw.IndexOf(',');
            if (commaIdx > 0 && raw.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                raw = raw.Substring(commaIdx + 1);

            return Convert.FromBase64String(raw);
        }

        /// <summary>把檔名 (可含副檔名 或不含) 正規化:lowercase, 空白→_, 去掉副檔名,固定加 .webp</summary>
        public static string NormalizeWebpFileName(string fileNameNoExt)
        {
            var name = (fileNameNoExt ?? "image").Trim();
            // 防呆:如果還是含副檔名,只取主檔名
            int dotIdx = name.LastIndexOf('.');
            if (dotIdx > 0) name = name.Substring(0, dotIdx);
            name = name.Replace(' ', '_').ToLowerInvariant();
            if (string.IsNullOrEmpty(name)) name = "image";
            return name + ".webp";
        }
    }
}
