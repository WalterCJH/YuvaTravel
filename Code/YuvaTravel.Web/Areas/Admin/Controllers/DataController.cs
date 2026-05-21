using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YuvaTravel.Web.Areas.Admin.Filter;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Entities;
using YuvaTravel.Data.Uow.IRepositories;
using YuvaTravel.Infrastructure.Helpers;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    public class DataController : BaseController
    {
        ITagRepository repo;
        IArticleCategoryRepository repoArticleCategory;
        IArticleRepository repoArticle;
        IIpBlockingRepository repoIpBlocking;
        IWebHostEnvironment _env;

        public DataController(IUnitOfWork unitOfWork, IWebHostEnvironment env) : base(unitOfWork)
        {
            repo = uow.TagRepo;
            repoArticleCategory = uow.ArticleCategoryRepo;
            repoArticle = uow.ArticleRepo;
            repoIpBlocking = uow.IpBlockingRepo;
            _env = env;
        }

        public IActionResult Init()
        {
            List<string> categories = new List<string>() { "旅遊", "訂房", "包車" };
            List<string> codes = new List<string>() { "Travel", "Booking", "RentCar" };
            for (int i = 0; i < categories.Count(); i++)
            {
                ArticleCategory articleCategory = new ArticleCategory();
                repoArticleCategory.Add(articleCategory);
                articleCategory.ArticleCategoryId = Guid.NewGuid();
                articleCategory.Name = categories[i];
                articleCategory.Code = codes[i];
                articleCategory.DisplaySeq = i + 1;
            }

            //List<string> tags = new List<string>() { "Tag1", "", "", "", "", "", "", "", "" };
            for (int i = 0; i < 10; i++)
            {
                Tag tag = new Tag();
                repo.Add(tag);
                tag.TagId = Guid.NewGuid();
                tag.ColorCode = "#003FDD";
                tag.Name = $"Tag{i + 1}";
                tag.Description = $"Tag{i + 1}-Test";
                tag.DisplaySeq = i;
            }

            uow.Commit();

            return RedirectToAction("Index", "Dashbroad");
        }


        public IActionResult GenArticleOnlineTime()
        {
            var articles = repoArticle.All().ToList();

            foreach (var article in articles)
            {
                article.OnlineTime = (article.OnScheduleTime > article.ReviewTime) ? article.OnScheduleTime : article.ReviewTime;
            }

            uow.Commit();

            return RedirectToAction("Index", "Dashbroad");
        }

        public async Task<IActionResult> GetArticleMobileFileName()
        {
            var articles = repoArticle.All().ToList();
            foreach (var article in articles)
            {
                if (article.ImagePath == null)
                    continue;
                //"/Images/Articles/1690/20220314-top電競-04.jpg"
                var tmp = article.ImagePath.Split('/');

                string[] folderPaths = new string[] { Folder.Articles, article.SerialNumber.ToString() };
                var newFileName = FileHelper.MappingFileNameEnd(tmp[tmp.Length - 1], "-m");
                var imagePath = _env.WebRootPath + article.ImagePath.Replace("/", "\\");
                imagePath = imagePath.Replace("\\\\", "\\");
                //var imagePath = Path.Combine(_env.WebRootPath, article.ImageHomePath.Replace("/", "\\"));
                //article.ImageMobilePath = FileHelper.SaveImageFromBase64(dto.ImageFileBase64, _env.WebRootPath, ImageSize.ArticleMobileWidth, 0, false, 80, folderPaths, newFileName, "jpeg");
                article.ImageMobilePath = FileHelper.SaveImage(imagePath, _env.WebRootPath, ImageSize.ArticleMobileWidth, 0, false, 80, folderPaths, newFileName);
                await uow.CommitAsync();
            }

            return RedirectToAction("Index", "Dashbroad");
        }

        //public IActionResult RC()
        //{
        //    var articles = repoArticle.All().Where(p => p.Content.Contains("widget-toc")).OrderBy(p => p.DisplaySeq).ToList();

        //    foreach (var article in articles)
        //    {
        //        if (article.Content.Contains("<div class=\"article-menu-box-list\""))
        //            continue;
        //        //if (article.Code == "sports-lottery-nba")
        //        //    ;
        //        string content = article.Content;

        //        var startIndex = content.IndexOf("<div class=\"widget-toc\">");
        //        if (startIndex < 0)
        //            startIndex = content.IndexOf("<div class=\"float-left widget-toc\">");
        //        if (startIndex < 0)
        //            startIndex = content.IndexOf("<div class=\"widget-toc float-left\">");
        //        //if (startIndex < 0)
        //        //    ;
        //        var LastIndex = content.IndexOf("</div>", startIndex) + "</div>".Length;

        //        var articleContent = content.Substring(startIndex, LastIndex - startIndex); // 文章目錄Html

        //        var olStartIndex = articleContent.IndexOf("<ol>");
        //        articleContent = articleContent.Trim().Remove(0, olStartIndex + "<ol>".Length); // 移除前面的html 到<ol>
        //        articleContent = "<ol class=\"article-menu-box-list\">\r\n" + articleContent;

        //        var temp1 = "<div class=\"article-menu-wrap\">\r\n" +
        //            "<button aria-controls=\"collapseExample\" aria-expanded=\"false\" class=\"btn article-menu-box-btn\" data-bs-target=\"#collapseExample\" data-bs-toggle=\"collapse\" type=\"button\"><em class=\"bi bi-list-ul\"></em> 文章目錄<em class=\"bi bi-chevron-expand\"></em></button>\r\n" +
        //            "<div class=\"article-menu-box collapse\" id=\"collapseExample\">\r\n";
        //        articleContent = temp1 + articleContent;
        //        articleContent = articleContent.Insert(articleContent.Length, "\r\n</div>"); // 增加 </div>

        //        articleContent = articleContent.Replace("<li>", "<li class=\"js-content-click\">");

        //        article.Content = article.Content.Remove(startIndex, LastIndex - startIndex); // 刪除原文章目錄整段html
        //        article.Content = article.Content.Insert(startIndex, articleContent); // 插入文章目錄到原content

        //    }

        //    uow.Commit();

        //    return RedirectToAction("Index", "Dashbroad");
        //}

        public IActionResult ImagePathFix()
        {
            //string sourcePath = _env.WebRootPath + ("~/Images/Articles/0/");
            string articlesDirPath = _env.WebRootPath + ("~/Images/Articles/");

            string wrongPath = "/Images/Articles/0/";
            var articles = repoArticle.All().Where(p => p.ImagePath.Contains(wrongPath)).OrderBy(p => p.SerialNumber).ToList();

            foreach (var article in articles)
            {
                string sourceFile1 = _env.WebRootPath + ("~" + article.ImagePath);
                string sourceFile2 = _env.WebRootPath + ("~" + article.ImageHomePath);
                string sourceFile3 = _env.WebRootPath + ("~" + article.ImageCategoryPath);

                string newPath = $"/Images/Articles/{article.SerialNumber}/";
                article.ImagePath = article.ImagePath.Replace(wrongPath, newPath);
                article.ImageHomePath = article.ImageHomePath.Replace(wrongPath, newPath);
                article.ImageCategoryPath = article.ImageCategoryPath.Replace(wrongPath, newPath);

                string targetFile1 = _env.WebRootPath + ("~" + article.ImagePath);
                string targetFile2 = _env.WebRootPath + ("~" + article.ImageHomePath);
                string targetFile3 = _env.WebRootPath + ("~" + article.ImageCategoryPath);

                System.IO.Directory.CreateDirectory($"{articlesDirPath}{article.SerialNumber}");
                System.IO.File.Move(sourceFile1, targetFile1);
                System.IO.File.Move(sourceFile2, targetFile2);
                System.IO.File.Move(sourceFile3, targetFile3);

                uow.Commit();
            }

            return RedirectToAction("Index", "Dashbroad");
        }

        public IActionResult RemoveImageWidth()
        {
            var articles = repoArticle.All().Where(p => p.Content.Contains("<img ") && p.Content.Contains("width")).ToList();

            foreach (var article in articles)
            {
                var flag = true;
                string tmpContent = article.Content;
                while (tmpContent.Contains("width"))
                {
                    var startIndex = tmpContent.IndexOf("style=\"width");
                    if (startIndex == -1)
                    {
                        flag = false;
                        break;
                    }
                    var lastIndex = tmpContent.IndexOf("px;\"", startIndex);
                    if (lastIndex == -1)
                    {
                        flag = false;
                        break;
                    }
                    tmpContent = tmpContent.Remove(startIndex, lastIndex - startIndex + 5);
                }

                if (flag)
                {
                    article.Content = tmpContent;
                    uow.Commit();
                }
            }

            return RedirectToAction("Index", "Dashbroad");
        }


        public IActionResult ImagePathEmptyFix()
        {
            string articlesDirPath = _env.WebRootPath + ("~/Images/Articles/");

            var articles = repoArticle.All().Where(p => p.ImagePath.Contains(" ")).OrderBy(p => p.SerialNumber).ToList();

            foreach (var article in articles)
            {
                logger.LogTrace("ArticleCode：{Code}", article.Code);
                string sourceFile1 = _env.WebRootPath + ("~" + article.ImagePath);
                string sourceFile2 = _env.WebRootPath + ("~" + article.ImageHomePath);
                string sourceFile3 = _env.WebRootPath + ("~" + article.ImageCategoryPath);
                string sourceFile4 = _env.WebRootPath + ("~" + article.ImageMobilePath);
                logger.LogTrace("sourceFile1：{File}", sourceFile1);
                logger.LogTrace("sourceFile2：{File}", sourceFile2);
                logger.LogTrace("sourceFile3：{File}", sourceFile3);
                logger.LogTrace("sourceFile4：{File}", sourceFile4);

                article.ImagePath = article.ImagePath.Replace(" ", "_");
                article.ImageHomePath = article.ImageHomePath.Replace(" ", "_");
                article.ImageCategoryPath = article.ImageCategoryPath.Replace(" ", "_");
                article.ImageMobilePath = article.ImageMobilePath.Replace(" ", "_");

                string targetFile1 = _env.WebRootPath + ("~" + article.ImagePath);
                string targetFile2 = _env.WebRootPath + ("~" + article.ImageHomePath);
                string targetFile3 = _env.WebRootPath + ("~" + article.ImageCategoryPath);
                string targetFile4 = _env.WebRootPath + ("~" + article.ImageMobilePath);
                logger.LogTrace("targetFile1：{File}", targetFile1);
                logger.LogTrace("targetFile2：{File}", targetFile2);
                logger.LogTrace("targetFile3：{File}", targetFile3);
                logger.LogTrace("targetFile4：{File}", targetFile4);

                System.IO.Directory.CreateDirectory($"{articlesDirPath}{article.SerialNumber}");
                if (!System.IO.File.Exists(targetFile1))
                    System.IO.File.Move(sourceFile1, targetFile1);
                if (!System.IO.File.Exists(targetFile2))
                    System.IO.File.Move(sourceFile2, targetFile2);
                if (!System.IO.File.Exists(targetFile3))
                    System.IO.File.Move(sourceFile3, targetFile3);
                if (!System.IO.File.Exists(targetFile4))
                    System.IO.File.Move(sourceFile4, targetFile4);

                uow.Commit();
            }

            return RedirectToAction("Index", "Dashbroad");
        }



        public async Task<IActionResult> AddGoogleCloudPlatform()
        {
            var json = @"{
  ""syncToken"": ""1661101279235"",
  ""creationTime"": ""2022-08-21T10:01:19.235766"",
  ""prefixes"": [{
                ""ipv4Prefix"": ""34.80.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
  }, {
                ""ipv4Prefix"": ""34.137.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.185.128.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.185.160.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.187.144.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.189.160.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.194.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.201.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.206.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.220.32.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.221.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.229.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.234.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.235.16.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.236.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""35.242.32.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""104.155.192.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""104.155.224.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""104.199.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""104.199.192.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""104.199.224.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""104.199.242.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""104.199.244.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""104.199.248.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""107.167.176.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""130.211.240.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv6Prefix"": ""2600:1900:4030::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east1""
         }, {
                ""ipv4Prefix"": ""34.92.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv4Prefix"": ""34.96.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv4Prefix"": ""34.104.88.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv4Prefix"": ""34.124.24.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv4Prefix"": ""34.150.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv4Prefix"": ""35.215.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv4Prefix"": ""35.220.27.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv4Prefix"": ""35.220.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv4Prefix"": ""35.241.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv4Prefix"": ""35.242.27.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv4Prefix"": ""35.243.8.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv6Prefix"": ""2600:1900:41a0::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-east2""
         }, {
                ""ipv4Prefix"": ""34.84.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""34.85.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""34.104.62.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""34.104.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""34.127.190.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""34.146.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""34.157.64.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""34.157.164.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""34.157.192.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""35.187.192.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""35.189.128.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""35.190.224.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""35.194.96.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""35.200.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""35.213.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""35.220.56.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""35.221.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""35.230.240.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""35.242.56.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""35.243.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""104.198.80.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""104.198.112.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv6Prefix"": ""2600:1900:4050::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast1""
         }, {
                ""ipv4Prefix"": ""34.97.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast2""
         }, {
                ""ipv4Prefix"": ""34.104.49.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast2""
         }, {
                ""ipv4Prefix"": ""34.127.177.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast2""
         }, {
                ""ipv4Prefix"": ""35.217.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast2""
         }, {
                ""ipv4Prefix"": ""35.220.45.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast2""
         }, {
                ""ipv4Prefix"": ""35.242.45.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast2""
         }, {
                ""ipv4Prefix"": ""35.243.56.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast2""
         }, {
                ""ipv6Prefix"": ""2600:1900:41d0::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast2""
         }, {
                ""ipv4Prefix"": ""34.64.32.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.64.64.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.64.68.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.64.72.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.64.80.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.64.96.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.64.128.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.64.132.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.64.136.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.64.144.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.64.160.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.64.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""35.216.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv6Prefix"": ""2600:1901:8180::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-northeast3""
         }, {
                ""ipv4Prefix"": ""34.93.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""34.100.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""34.104.108.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""34.124.44.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""34.157.87.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""34.157.215.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""35.200.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""35.201.41.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""35.207.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""35.220.42.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""35.234.208.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""35.242.42.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""35.244.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv6Prefix"": ""2600:1900:40a0::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south1""
         }, {
                ""ipv4Prefix"": ""34.104.120.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south2""
         }, {
                ""ipv4Prefix"": ""34.124.56.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south2""
         }, {
                ""ipv4Prefix"": ""34.126.208.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south2""
         }, {
                ""ipv4Prefix"": ""34.131.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south2""
         }, {
                ""ipv6Prefix"": ""2600:1900:41b0::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-south2""
         }, {
                ""ipv4Prefix"": ""34.87.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.87.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.104.58.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.104.106.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.124.42.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.124.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.126.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.126.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.142.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.143.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.157.82.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.157.88.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.157.210.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.185.176.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.186.144.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.187.224.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.197.128.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.198.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.213.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.220.24.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.234.192.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.240.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.242.24.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.247.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv6Prefix"": ""2600:1900:4080::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.101.18.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast2""
         }, {
                ""ipv4Prefix"": ""34.101.20.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast2""
         }, {
                ""ipv4Prefix"": ""34.101.24.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast2""
         }, {
                ""ipv4Prefix"": ""34.101.32.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast2""
         }, {
                ""ipv4Prefix"": ""34.101.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast2""
         }, {
                ""ipv4Prefix"": ""34.101.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast2""
         }, {
                ""ipv4Prefix"": ""34.128.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast2""
         }, {
                ""ipv4Prefix"": ""35.219.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast2""
         }, {
                ""ipv6Prefix"": ""2600:1901:8170::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""asia-southeast2""
         }, {
                ""ipv4Prefix"": ""34.87.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.104.104.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.116.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.124.40.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.151.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.151.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.189.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.197.160.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.201.0.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.213.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.220.41.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.234.224.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.242.41.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""35.244.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv6Prefix"": ""2600:1900:40b0::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast1""
         }, {
                ""ipv4Prefix"": ""34.104.122.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast2""
         }, {
                ""ipv4Prefix"": ""34.124.58.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast2""
         }, {
                ""ipv4Prefix"": ""34.126.192.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast2""
         }, {
                ""ipv4Prefix"": ""34.129.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast2""
         }, {
                ""ipv6Prefix"": ""2600:1900:41c0::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""australia-southeast2""
         }, {
                ""ipv4Prefix"": ""34.104.116.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-central2""
         }, {
                ""ipv4Prefix"": ""34.116.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-central2""
         }, {
                ""ipv4Prefix"": ""34.118.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-central2""
         }, {
                ""ipv4Prefix"": ""34.124.52.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-central2""
         }, {
                ""ipv6Prefix"": ""2600:1900:4140::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-central2""
         }, {
                ""ipv4Prefix"": ""34.88.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-north1""
         }, {
                ""ipv4Prefix"": ""34.104.96.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-north1""
         }, {
                ""ipv4Prefix"": ""34.124.32.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-north1""
         }, {
                ""ipv4Prefix"": ""35.203.232.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-north1""
         }, {
                ""ipv4Prefix"": ""35.217.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-north1""
         }, {
                ""ipv4Prefix"": ""35.220.26.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-north1""
         }, {
                ""ipv4Prefix"": ""35.228.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-north1""
         }, {
                ""ipv4Prefix"": ""35.242.26.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-north1""
         }, {
                ""ipv6Prefix"": ""2600:1900:4150::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-north1""
         }, {
                ""ipv4Prefix"": ""34.157.44.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-southwest1""
         }, {
                ""ipv4Prefix"": ""34.157.172.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-southwest1""
         }, {
                ""ipv4Prefix"": ""34.164.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-southwest1""
         }, {
                ""ipv4Prefix"": ""34.175.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-southwest1""
         }, {
                ""ipv6Prefix"": ""2600:1901:8100::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-southwest1""
         }, {
                ""ipv4Prefix"": ""8.34.208.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""8.34.211.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""8.34.220.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""23.251.128.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""34.76.0.0/14"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""34.118.254.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""34.140.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.187.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.187.160.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.189.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.190.192.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.195.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.205.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.206.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.210.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.220.96.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.233.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.240.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.241.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""35.242.64.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""104.155.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""104.199.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""104.199.66.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""104.199.68.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""104.199.72.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""104.199.80.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""104.199.96.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""130.211.48.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""130.211.64.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""130.211.96.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""146.148.2.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""146.148.4.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""146.148.8.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""146.148.16.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""146.148.112.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""192.158.28.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv6Prefix"": ""2600:1900:4010::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west1""
         }, {
                ""ipv4Prefix"": ""34.89.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""34.105.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""34.127.186.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""34.142.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""34.147.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""34.157.36.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""34.157.40.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""34.157.168.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.189.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.197.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.203.210.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.203.212.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.203.216.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.214.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.220.20.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.230.128.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.234.128.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.235.48.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.242.20.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.242.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""35.246.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv6Prefix"": ""2600:1900:40c0::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west2""
         }, {
                ""ipv4Prefix"": ""34.89.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""34.104.112.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""34.107.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""34.118.244.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""34.124.48.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""34.141.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""34.157.48.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""34.157.176.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""34.159.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""35.198.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""35.198.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""35.207.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""35.207.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""35.220.18.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""35.234.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""35.235.32.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""35.242.18.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""35.242.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""35.246.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv6Prefix"": ""2600:1900:40d0::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west3""
         }, {
                ""ipv4Prefix"": ""34.90.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""34.104.126.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""34.124.62.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""34.141.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""34.147.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""34.157.80.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""34.157.92.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""34.157.208.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""34.157.220.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""35.204.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""35.214.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""35.220.16.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""35.234.160.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""35.242.16.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv6Prefix"": ""2600:1900:4060::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west4""
         }, {
                ""ipv4Prefix"": ""34.65.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west6""
         }, {
                ""ipv4Prefix"": ""34.104.110.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west6""
         }, {
                ""ipv4Prefix"": ""34.124.46.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west6""
         }, {
                ""ipv4Prefix"": ""35.216.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west6""
         }, {
                ""ipv4Prefix"": ""35.220.44.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west6""
         }, {
                ""ipv4Prefix"": ""35.235.216.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west6""
         }, {
                ""ipv4Prefix"": ""35.242.44.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west6""
         }, {
                ""ipv6Prefix"": ""2600:1900:4160::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west6""
         }, {
                ""ipv4Prefix"": ""34.154.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west8""
         }, {
                ""ipv4Prefix"": ""34.157.8.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west8""
         }, {
                ""ipv4Prefix"": ""34.157.136.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west8""
         }, {
                ""ipv4Prefix"": ""35.219.224.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west8""
         }, {
                ""ipv6Prefix"": ""2600:1901:8110::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west8""
         }, {
                ""ipv4Prefix"": ""34.155.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west9""
         }, {
                ""ipv4Prefix"": ""34.157.12.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west9""
         }, {
                ""ipv4Prefix"": ""34.157.140.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west9""
         }, {
                ""ipv4Prefix"": ""34.163.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west9""
         }, {
                ""ipv6Prefix"": ""2600:1901:8120::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""europe-west9""
         }, {
                ""ipv4Prefix"": ""34.95.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.96.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.98.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.102.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.104.27.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.107.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.110.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.111.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.116.0.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.117.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.120.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.128.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.144.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.149.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.160.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""35.186.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""35.190.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""35.190.64.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""35.190.112.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""35.201.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""35.227.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""35.241.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""35.244.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""107.178.240.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""130.211.4.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""130.211.8.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""130.211.16.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""130.211.32.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv6Prefix"": ""2600:1901::/48"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv6Prefix"": ""2600:1901:1:1000::/52"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv6Prefix"": ""2600:1901:1:2000::/51"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv6Prefix"": ""2600:1901:1:4000::/50"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv6Prefix"": ""2600:1901:1:8000::/49"",
    ""service"": ""Google Cloud"",
    ""scope"": ""global""
         }, {
                ""ipv4Prefix"": ""34.95.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast1""
         }, {
                ""ipv4Prefix"": ""34.104.76.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast1""
         }, {
                ""ipv4Prefix"": ""34.118.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast1""
         }, {
                ""ipv4Prefix"": ""34.124.12.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast1""
         }, {
                ""ipv4Prefix"": ""34.152.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast1""
         }, {
                ""ipv4Prefix"": ""35.203.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast1""
         }, {
                ""ipv4Prefix"": ""35.215.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast1""
         }, {
                ""ipv4Prefix"": ""35.220.43.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast1""
         }, {
                ""ipv4Prefix"": ""35.234.240.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast1""
         }, {
                ""ipv4Prefix"": ""35.242.43.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast1""
         }, {
                ""ipv6Prefix"": ""2600:1900:40e0::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast1""
         }, {
                ""ipv4Prefix"": ""34.104.114.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast2""
         }, {
                ""ipv4Prefix"": ""34.124.50.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast2""
         }, {
                ""ipv4Prefix"": ""34.124.112.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast2""
         }, {
                ""ipv4Prefix"": ""34.130.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast2""
         }, {
                ""ipv6Prefix"": ""2600:1900:41e0::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""northamerica-northeast2""
         }, {
                ""ipv4Prefix"": ""34.95.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""34.104.80.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""34.124.16.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""34.151.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""34.151.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""35.198.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""35.199.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""35.215.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""35.220.40.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""35.235.0.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""35.242.40.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""35.247.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv6Prefix"": ""2600:1900:40f0::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-east1""
         }, {
                ""ipv4Prefix"": ""34.104.50.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-west1""
         }, {
                ""ipv4Prefix"": ""34.127.178.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-west1""
         }, {
                ""ipv4Prefix"": ""34.176.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-west1""
         }, {
                ""ipv6Prefix"": ""2600:1901:4010::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""southamerica-west1""
         }, {
                ""ipv4Prefix"": ""8.34.210.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""8.34.212.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""8.34.216.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""8.35.192.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""23.236.48.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""23.251.144.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.16.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.66.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.68.0.0/14"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.72.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.118.200.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.121.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.122.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.132.0.0/14"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.136.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.157.84.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.157.96.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.157.212.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.157.224.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.170.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""34.172.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.184.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.188.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.188.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.188.192.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.192.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.194.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.202.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.206.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.208.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.220.64.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.222.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.224.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.226.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.232.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.238.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.242.96.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.154.16.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.154.32.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.154.64.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.154.96.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.154.113.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.154.114.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.154.116.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.154.120.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.154.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.155.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.197.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.198.16.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.198.32.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.198.64.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""104.198.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""107.178.208.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""108.59.80.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""130.211.112.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""130.211.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""130.211.192.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""130.211.224.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""146.148.32.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""146.148.64.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""146.148.96.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""162.222.176.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""173.255.112.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""199.192.115.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""199.223.232.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""199.223.236.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv6Prefix"": ""2600:1900:4000::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central1""
         }, {
                ""ipv4Prefix"": ""35.186.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central2""
         }, {
                ""ipv4Prefix"": ""35.186.128.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central2""
         }, {
                ""ipv4Prefix"": ""35.206.32.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central2""
         }, {
                ""ipv4Prefix"": ""35.220.46.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central2""
         }, {
                ""ipv4Prefix"": ""35.242.46.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central2""
         }, {
                ""ipv4Prefix"": ""107.167.160.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central2""
         }, {
                ""ipv4Prefix"": ""108.59.88.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central2""
         }, {
                ""ipv4Prefix"": ""173.255.120.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central2""
         }, {
                ""ipv6Prefix"": ""2600:1900:4070::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-central2""
         }, {
                ""ipv4Prefix"": ""34.73.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""34.74.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""34.98.128.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""34.118.250.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""34.138.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""34.148.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.185.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.190.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.196.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.207.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.211.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.220.0.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.227.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.229.16.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.229.32.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.229.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.231.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.237.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.242.0.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""35.243.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""104.196.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""104.196.65.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""104.196.66.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""104.196.68.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""104.196.96.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""104.196.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""104.196.192.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""162.216.148.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv6Prefix"": ""2600:1900:4020::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east1""
         }, {
                ""ipv4Prefix"": ""34.85.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.86.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.104.60.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.104.124.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.118.252.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.124.60.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.127.188.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.145.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.150.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.157.0.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.157.16.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.157.128.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.157.144.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.186.160.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.188.224.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.194.64.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.199.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.212.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.220.60.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.221.0.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.230.160.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.234.176.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.236.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.242.60.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.243.40.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""35.245.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv6Prefix"": ""2600:1900:4090::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east4""
         }, {
                ""ipv4Prefix"": ""34.157.32.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east5""
         }, {
                ""ipv4Prefix"": ""34.157.160.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east5""
         }, {
                ""ipv4Prefix"": ""34.162.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east5""
         }, {
                ""ipv6Prefix"": ""2600:1901:8130::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east5""
         }, {
                ""ipv4Prefix"": ""34.104.56.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east7""
         }, {
                ""ipv4Prefix"": ""34.127.184.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east7""
         }, {
                ""ipv4Prefix"": ""34.161.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east7""
         }, {
                ""ipv4Prefix"": ""35.206.10.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east7""
         }, {
                ""ipv6Prefix"": ""2600:1901:8150::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-east7""
         }, {
                ""ipv4Prefix"": ""34.157.46.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-south1""
         }, {
                ""ipv4Prefix"": ""34.157.174.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-south1""
         }, {
                ""ipv4Prefix"": ""34.174.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-south1""
         }, {
                ""ipv6Prefix"": ""2600:1901:8140::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-south1""
         }, {
                ""ipv4Prefix"": ""34.82.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""34.105.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""34.118.192.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""34.127.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""34.145.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""34.157.112.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""34.157.240.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""34.168.0.0/15"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.185.192.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.197.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.199.144.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.199.160.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.203.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.212.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.220.48.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.227.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.230.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.233.128.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.242.48.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.243.32.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""35.247.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""104.196.224.0/19"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""104.198.0.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""104.198.96.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""104.199.112.0/20"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv6Prefix"": ""2600:1900:4040::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west1""
         }, {
                ""ipv4Prefix"": ""34.94.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""34.102.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""34.104.64.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""34.108.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""34.118.248.0/23"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""34.124.0.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""35.215.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""35.220.47.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""35.235.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""35.236.0.0/17"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""35.242.47.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""35.243.0.0/21"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv6Prefix"": ""2600:1900:4120::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west2""
         }, {
                ""ipv4Prefix"": ""34.104.52.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west3""
         }, {
                ""ipv4Prefix"": ""34.106.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west3""
         }, {
                ""ipv4Prefix"": ""34.127.180.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west3""
         }, {
                ""ipv4Prefix"": ""35.217.64.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west3""
         }, {
                ""ipv4Prefix"": ""35.220.31.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west3""
         }, {
                ""ipv4Prefix"": ""35.242.31.0/24"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west3""
         }, {
                ""ipv6Prefix"": ""2600:1900:4170::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west3""
         }, {
                ""ipv4Prefix"": ""34.104.72.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west4""
         }, {
                ""ipv4Prefix"": ""34.118.240.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west4""
         }, {
                ""ipv4Prefix"": ""34.124.8.0/22"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west4""
         }, {
                ""ipv4Prefix"": ""34.125.0.0/16"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west4""
         }, {
                ""ipv4Prefix"": ""35.219.128.0/18"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west4""
         }, {
                ""ipv6Prefix"": ""2600:1900:4180::/44"",
    ""service"": ""Google Cloud"",
    ""scope"": ""us-west4""
         }]
}";

            var result = JsonConvert.DeserializeObject<Rootobject>(json);

            foreach (var prefix in result.prefixes)
            {
                var tmpIP = "";
                if (!string.IsNullOrWhiteSpace(prefix.ipv4Prefix))
                {
                    tmpIP = prefix.ipv4Prefix;
                }
                else
                {
                    tmpIP = prefix.ipv6Prefix;
                }

                var tmp = tmpIP.Split('/');
                var ip = tmp[0];

                if (!string.IsNullOrWhiteSpace(ip))
                {
                    var ipBlocking = repoIpBlocking.All().FirstOrDefault(p => p.IpAddress == ip);
                    if (ipBlocking == null)
                    {
                        ipBlocking = new IpBlocking();
                        repoIpBlocking.Add(ipBlocking);
                        ipBlocking.IpBlockingId = Guid.NewGuid();
                        ipBlocking.IsActive = true;
                        ipBlocking.IpAddress = ip;
                        ipBlocking.Memo = "GCP";
                        ipBlocking.UserLog.CreateTime = DateTime.Now;
                        ipBlocking.UserLog.CreateUserId = "system";
                        await uow.CommitAsync();
                    }
                }
            }

            return RedirectToAction("Index", "Dashbroad");
        }


        public class Rootobject
        {
            public string syncToken { get; set; }
            public DateTime creationTime { get; set; }
            public Prefix[] prefixes { get; set; }
        }

        public class Prefix
        {
            public string ipv4Prefix { get; set; }
            public string service { get; set; }
            public string scope { get; set; }
            public string ipv6Prefix { get; set; }
        }


    }
}