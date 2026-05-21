using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Images;
using YuvaTravel.Infrastructure.Helpers;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    public class ImagesController : BaseController
    {
        IWebHostEnvironment _env;
        ILogger<ImagesController> _logger;

        public ImagesController(
            IWebHostEnvironment env,
            ILogger<ImagesController> logger,
            YuvaTravel.Data.Uow.IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            _env = env;
            _logger = logger;
        }

        public IActionResult CKEditorImageFileManager(string CKEditorFuncNum)
        {
            CKEditorFuncNum = (CKEditorFuncNum == null) ? string.Empty : CKEditorFuncNum.Trim();
            ViewData["CKEditorFuncNum"] = CKEditorFuncNum;

            string root = Path.Combine(_env.WebRootPath, StrPath.ImageUserFiles);
            if(!Directory.Exists(root)) Directory.CreateDirectory(root);
            DirectoryInfo dirInfo = new DirectoryInfo(root);
            var searchFiles = dirInfo.EnumerateFiles("*.*", SearchOption.AllDirectories)
                .Where(o => o.Extension.ToLower().Equals(".jpg") || o.Extension.ToLower().Equals(".jpeg") || o.Extension.ToLower().Equals(".gif") || o.Extension.ToLower().Equals(".png") || o.Extension.ToLower().Equals(".webp"));

            var fileDatas = searchFiles.OrderByDescending(o => o.LastWriteTime).Take(20).Select(o =>
            {
                var fileName = Path.GetFileName(o.FullName);
                return $"/{StrPath.ImageUserFiles}/{fileName}";
            }).ToList();

            return View(fileDatas);
        }

        public IActionResult CKEditorVideoFileManager(string CKEditorFuncNum, string videoType)
        {
            CKEditorFuncNum = (CKEditorFuncNum == null) ? string.Empty : CKEditorFuncNum.Trim();
            videoType = (videoType == null) ? string.Empty : videoType.Trim();
            List<string> fileDatas = new List<string>();
            if (Enum.TryParse<VideoType>(videoType, out VideoType videoTypeValue))
            {
                ViewData["CKEditorFuncNum"] = CKEditorFuncNum;
                ViewData["videoType"] = videoType;

                string root = Path.Combine(_env.WebRootPath, StrPath.ImageUserVideos);
                DirectoryInfo dirInfo = new DirectoryInfo(root);
                var searchFiles = dirInfo.EnumerateFiles("*.*", SearchOption.AllDirectories);
                switch (videoTypeValue)
                {
                    case VideoType.Mp4:
                        searchFiles = searchFiles
                            .Where(o => o.Extension.ToLower().Equals(".mp4") || o.Extension.ToLower().Equals(".mov"));
                        break;
                    case VideoType.WebM:
                        searchFiles = searchFiles
                            .Where(o => o.Extension.ToLower().Equals(".webm"));
                        break;
                }

                fileDatas = searchFiles.OrderByDescending(o => o.LastWriteTime).Select(o =>
                {
                    var fileName = Path.GetFileName(o.FullName);
                    return $"/{StrPath.ImageUserVideos}/{fileName}";
                }).ToList();
            }

            return View(fileDatas);
        }

        public async Task<IActionResult> CKEditorFileUpload(IFormFile[] file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { error = "請選擇檔案.", errorkeys = new string[0] });
            }

            string errorMessage = "總共有 {0} 個錯誤檔案";
            string root = Path.Combine(_env.WebRootPath, StrPath.ImageUserFiles);
            Directory.CreateDirectory(root);
            List<string> errorKeys = new List<string>();
            List<Tuple<string, string>> files = new List<Tuple<string, string>>();

            for (int i = 0; i < file.Length; i++)
            {
                var fileItem = file[i];
                string fileExtension = Path.GetExtension(fileItem.FileName);
                string fileName = Path.GetFileNameWithoutExtension(fileItem.FileName);
                fileName = fileName.Replace(" ", "_");

                var checkFiles = Directory.GetFiles(root, fileItem.FileName);
                if (checkFiles != null && checkFiles.Length > 0)
                {
                    Random random = new Random(DateTime.Now.Second);
                    var sno = random.Next(0, 10000);
                    fileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}_{sno}{fileExtension}";
                }
                else
                {
                    fileName = $"{fileName}{fileExtension}";
                }

                string fileFullName = Path.Combine(root, fileName);
                try
                {
                    using (var stream = System.IO.File.Create(fileFullName))
                    {
                        await fileItem.CopyToAsync(stream);
                    }
                    FileHelper.VaryQualityLevel(fileFullName, fileFullName, 80);
                    files.Add(new Tuple<string, string>($"/{StrPath.ImageUserFiles}/", fileName));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CKEditor 上傳失敗:{FileName}", fileItem.FileName);
                    try { if (System.IO.File.Exists(fileFullName)) System.IO.File.Delete(fileFullName); } catch { /* 失敗也忽略 */ }
                    errorKeys.Add(i.ToString());
                    continue;
                }
            }

            if (errorKeys.Count > 0)
            {
                return Json(new { error = string.Format(errorMessage, errorKeys.Count), errorkeys = errorKeys.ToArray() });
            }
            else
            {
                return Json(new { files = files.Select(o => new { FilePath = o.Item1, FileName = o.Item2 }).ToArray() });
            }
        }

        public IActionResult CKEditorFileDelete(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return Json(new { IsSuccess = false, Message = "File name must input." });

            string root = Path.Combine(_env.WebRootPath, StrPath.ImageUserFiles);
            DirectoryInfo dirInfo = new DirectoryInfo(root);
            var checkFiles = dirInfo.GetFiles(fileName);
            if (checkFiles != null && checkFiles.Length > 0)
            {
                foreach (var item in checkFiles)
                {
                    try
                    {
                        var delFileName = Path.GetFileName(item.FullName);
                        if (delFileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            item.Delete();
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                return Json(new { IsSuccess = true, Message = string.Empty });
            }
            else
            {
                return Json(new { IsSuccess = false, Message = "Can't find file by file name." });
            }
        }

        public async Task<IActionResult> Uploading(IFormFile upload)
        {
            string imageUrl = "";
            UploadImg uploadImg = new UploadImg();

            try
            {
                string directoryPath = StrPath.ImageUserFiles;
                string uploadPath = Path.Combine(_env.WebRootPath, directoryPath);
                Directory.CreateDirectory(uploadPath);

                string fileName = upload.FileName;
                string fileContentType = upload.ContentType;
                if (fileContentType == "image/bmp" || fileContentType == "image/gif" || fileContentType == "image/png" || fileContentType == "image/x-png" || fileContentType == "image/jpeg" || fileContentType == "image/pjpeg" || fileContentType == "image/webp")
                {
                    string fileFullNameTmp = Path.Combine(uploadPath, fileName).Replace(".", "-tmp.");
                    string fileFullName = Path.Combine(uploadPath, fileName);

                    using (var stream = System.IO.File.Create(fileFullNameTmp))
                    {
                        await upload.CopyToAsync(stream);
                    }
                    FileHelper.VaryQualityLevel(fileFullNameTmp, fileFullName, 70);
                    System.IO.File.Delete(fileFullNameTmp);

                    imageUrl = $"{Request.Scheme}://{Request.Host}/{directoryPath}/{fileName}";
                    uploadImg.uploaded = 1;
                    uploadImg.fileName = fileName;
                    uploadImg.url = imageUrl;
                    uploadImg.error = new UploadImgMsg() { message = "成功" };
                }
                else
                {
                    uploadImg.uploaded = 0;
                    uploadImg.error = new UploadImgMsg() { message = "只支援 BMP、GIF、JPG、PNG、WebP 格式的圖片!" };
                }
            }
            catch (Exception ex)
            {
                uploadImg.uploaded = 0;
                uploadImg.error = new UploadImgMsg() { message = $"上傳失敗，{ex.Message}" };
            }
            return Content(JsonConvert.SerializeObject(uploadImg));
        }
    }
}
