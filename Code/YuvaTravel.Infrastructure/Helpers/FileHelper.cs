using YuvaTravel.Base.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;

namespace YuvaTravel.Infrastructure.Helpers
{
    public class FileHelper
    {
        /// <summary>
        /// 壓縮圖片質量 (SkiaSharp)。支援 JPG/PNG/WebP/GIF。
        ///   - 先把原檔讀進 byte[] 再操作,避免 source = destination 造成檔案鎖死
        ///   - .gif 維持原始 byte 不重編碼,保留動畫
        ///   - .png 無損輸出 (品質 100)
        ///   - .webp / .jpg / .jpeg 依 qualityLevel 重新編碼
        /// </summary>
        /// <param name="imgSourceFilePath">圖片來源路徑</param>
        /// <param name="outImgFilePath">圖片目的路徑 (副檔名決定輸出格式)</param>
        /// <param name="qualityLevel">數字愈低,破壞性壓縮愈大 (1–100)</param>
        public static void VaryQualityLevel(string imgSourceFilePath, string outImgFilePath, long qualityLevel = 90L)
        {
            byte[] bytes = File.ReadAllBytes(imgSourceFilePath);

            var ext = Path.GetExtension(outImgFilePath).ToLowerInvariant();

            // GIF 直接 raw copy 保留動畫 (SkiaSharp 不支援寫 GIF)
            if (ext == ".gif")
            {
                if (!string.Equals(imgSourceFilePath, outImgFilePath, StringComparison.OrdinalIgnoreCase))
                {
                    File.WriteAllBytes(outImgFilePath, bytes);
                }
                return;
            }

            using var src = SKBitmap.Decode(bytes);
            if (src == null)
            {
                throw new InvalidOperationException($"圖片解碼失敗:{Path.GetFileName(imgSourceFilePath)}");
            }

            using var img = SKImage.FromBitmap(src);
            var (format, quality) = ext switch
            {
                ".png"  => (SKEncodedImageFormat.Png,  100),
                ".webp" => (SKEncodedImageFormat.Webp, (int)qualityLevel),
                _       => (SKEncodedImageFormat.Jpeg, (int)qualityLevel),
            };

            using var data = img.Encode(format, quality);
            if (data == null)
            {
                throw new InvalidOperationException($"圖片編碼失敗 ({format}):{Path.GetFileName(outImgFilePath)}");
            }
            using var fs = File.Create(outImgFilePath);
            data.SaveTo(fs);
        }

        /// <summary>
        /// 讀取圖片,依寬/高縮放後儲存 (SkiaSharp)。
        /// 縮放規則沿用舊版 ResizeImage:
        ///   - 只給 toHeight:原圖比 toHeight 高 (或 sizeReplace) 才依高度等比縮
        ///   - 只給 toWidth :原圖比 toWidth 寬 (或 sizeReplace) 才依寬度等比縮
        ///   - 兩者皆 0     :原尺寸重新編碼
        ///   - 其它情況     :不縮放,維持原圖
        /// 輸出格式依目的副檔名:.png 無損 / .webp / 其它走 JPEG,品質 = quality。
        /// </summary>
        public static string SaveImage(string ImagePath, string basePath, int toWidth, int toHeight, bool sizeReplace, long quality, string[] folderPaths, string newFileName = null)
        {
            string savedFolder = Path.Combine(basePath, Folder.Images);
            string returnPath = $"/{Folder.Images}";

            foreach (var folderPath in folderPaths)
            {
                if (!string.IsNullOrEmpty(folderPath))
                {
                    savedFolder = Path.Combine(savedFolder, folderPath);
                    returnPath = $"{returnPath}/{folderPath}";
                }
            }

            CheckDirectory(savedFolder);

            string fileName = string.IsNullOrEmpty(newFileName) ? Path.GetFileName(ImagePath) : newFileName;
            string filePath = Path.Combine(savedFolder, fileName);

            byte[] bytes = File.ReadAllBytes(ImagePath);
            using var src = SKBitmap.Decode(bytes);
            if (src == null)
            {
                throw new InvalidOperationException($"圖片解碼失敗:{Path.GetFileName(ImagePath)}");
            }

            var (targetW, targetH, doResize) = ComputeTargetSize(src.Width, src.Height, toWidth, toHeight, sizeReplace);

            SKBitmap working = src;
            bool ownWorking = false;
            try
            {
                if (doResize && (targetW != src.Width || targetH != src.Height))
                {
                    var info = new SKImageInfo(targetW, targetH, SKColorType.Bgra8888, SKAlphaType.Premul);
                    var resized = new SKBitmap(info);
                    var sampling = new SKSamplingOptions(SKCubicResampler.Mitchell);
                    if (!src.ScalePixels(resized, sampling))
                    {
                        resized.Dispose();
                        throw new InvalidOperationException($"圖片縮放失敗:{Path.GetFileName(ImagePath)}");
                    }
                    working = resized;
                    ownWorking = true;
                }

                var ext = Path.GetExtension(fileName).ToLowerInvariant();
                var (format, q) = ext switch
                {
                    ".png"  => (SKEncodedImageFormat.Png,  100),
                    ".webp" => (SKEncodedImageFormat.Webp, (int)quality),
                    _       => (SKEncodedImageFormat.Jpeg, (int)quality),
                };

                using var img = SKImage.FromBitmap(working);
                using var data = img.Encode(format, q);
                if (data == null)
                {
                    throw new InvalidOperationException($"圖片編碼失敗 ({format}):{fileName}");
                }
                using var fs = File.Create(filePath);
                data.SaveTo(fs);
            }
            finally
            {
                if (ownWorking) working.Dispose();
            }

            returnPath = $"{returnPath}/{fileName}";
            return returnPath;
        }

        /// <summary>
        /// 計算目標尺寸。回傳 (寬, 高, 是否需縮放)。沿用舊 ResizeImage 判斷邏輯。
        /// </summary>
        private static (int width, int height, bool resize) ComputeTargetSize(
            int srcWidth, int srcHeight, int toWidth, int toHeight, bool sizeReplace)
        {
            if (toWidth == 0 && toHeight != 0 && (srcHeight > toHeight || sizeReplace))
            {
                float tmpf = (float)srcHeight / toHeight;
                return ((int)(srcWidth / tmpf), toHeight, true);
            }
            if (toWidth != 0 && toHeight == 0 && (srcWidth > toWidth || sizeReplace))
            {
                float tmpf = (float)srcWidth / toWidth;
                return (toWidth, (int)(srcHeight / tmpf), true);
            }
            if (toWidth == 0 && toHeight == 0)
            {
                // 原尺寸重新編碼
                return (srcWidth, srcHeight, false);
            }
            // 其它情況不縮放
            return (srcWidth, srcHeight, false);
        }

        public static string MappingFileNameEnd(string oldFileName, string fileNameEnd)
        {
            var tmps = oldFileName.Split('.');
            string newFileName = "";
            for (int i = 0; i < tmps.Length - 1; i++)
            {
                newFileName += tmps[i];
            }
            newFileName += $"{fileNameEnd}.{tmps[tmps.Length - 1]}";

            return newFileName;
        }

        /// <summary>
        /// 檢查資料夾是否存在,若不存在則新增
        /// </summary>
        public static void CheckDirectory(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        /// <summary>
        /// 進行資料夾內所有圖片縮放,並產生新檔案
        /// </summary>
        public static Task ResizeImagesAsync(string sourcePath, string destPath, double scale, CancellationToken token = default)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            var allFiles = QueryImages(sourcePath);
            var allTask = new List<Task>();
            foreach (var filePath in allFiles)
            {
                //allTask.Add(ResizeImageAsync(filePath, destPath, scale));
            }

            return Task.WhenAll(allTask);
        }

        /// <summary>
        /// 清空目的目錄下的所有檔案與目錄
        /// </summary>
        public static void Clean(string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            else
            {
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);

                foreach (var item in allImageFiles)
                {
                    File.Delete(item);
                }
            }
        }

        /// <summary>
        /// 找出指定目錄下的圖片
        /// </summary>
        public static List<string> QueryImages(string srcPath)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(srcPath, "*.png", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpg", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpeg", SearchOption.AllDirectories));
            return files;
        }
    }
}
