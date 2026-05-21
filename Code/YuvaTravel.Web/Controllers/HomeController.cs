using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using X.PagedList.Extensions;
using YuvaTravel.Base.Constants;
using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Dtos.Home;
using YuvaTravel.Data.Entities;
using YuvaTravel.Data.Uow;
using YuvaTravel.Infrastructure.Helpers;
using YuvaTravel.Web.Helpers;
using Guide = YuvaTravel.Data.Entities.Guide;


namespace YuvaTravel.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// 統一接收前台所有訂閱表單 (newsletter / aside-newsletter ...),寫一筆 Subscriber 並導回首頁。
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscriber(string email, string sourcePath = null)
        {
            email = email?.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(email) ||
                !System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                TempData["subscribe-err"] = "請輸入有效的 Email";
                return RedirectToAction(nameof(Index));
            }

            var req = HttpContext.Request;

            var sourcePathFinal = !string.IsNullOrWhiteSpace(sourcePath)
                ? sourcePath
                : (req.Headers["Referer"].ToString() ?? "");
            string sourceUrl = null;
            try
            {
                if (Uri.TryCreate(sourcePathFinal, UriKind.Absolute, out var abs))
                    sourceUrl = abs.ToString();
                else if (!string.IsNullOrEmpty(sourcePathFinal))
                    sourceUrl = $"{req.Scheme}://{req.Host}{sourcePathFinal}";
            }
            catch { /* swallow,sourceUrl 留 null */ }

            var nowTime = DateTime.Now;

            // 已經有有效訂閱 → 直接拒,不寫入
            var active = await uow.SubscriberRepo.FindActiveByEmailAsync(email);
            if (active != null)
            {
                TempData["subscribe-info"] = $"{email} 已經訂閱過了,謝謝你的支持!";
                return RedirectToAction(nameof(Index));
            }

            var sub = new Subscriber
            {
                SubscriberId = Guid.NewGuid(),
                UnsubscribeToken = Guid.NewGuid(),
                Email = email,
                IsActive = true,
                SubscribeTime = nowTime,
                SourcePath = TrimTo(sourcePathFinal, 500),
                SourceUrl = TrimTo(sourceUrl, 1000),
                Referrer = TrimTo(req.Headers["Referer"].ToString(), 1000),
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = TrimTo(req.Headers["User-Agent"].ToString(), 500),
                AcceptLanguage = TrimTo(req.Headers["Accept-Language"].ToString(), 100),
                UtmSource = TrimTo(req.Query["utm_source"].ToString(), 100),
                UtmMedium = TrimTo(req.Query["utm_medium"].ToString(), 100),
                UtmCampaign = TrimTo(req.Query["utm_campaign"].ToString(), 100)
            };
            sub.UserLog.CreateTime = nowTime;
            sub.UserLog.CreateUserId = "Anonymous";
            uow.SubscriberRepo.Add(sub);
            await uow.CommitAsync();

            TempData["subscribe-ok"] = "感謝您的訂閱,我們會把每月一封慢慢寄到您的信箱。";
            return RedirectToAction(nameof(Index));
        }

        private static string TrimTo(string value, int max)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return value.Length > max ? value.Substring(0, max) : value;
        }

        [HttpGet]
        public async Task<IActionResult> Unsubscribe(Guid? t)
        {
            ViewBag.UnsubResult = "invalid";
            if (!t.HasValue || t == Guid.Empty)
                return View();

            // Token 流程要改 IsActive → tracking
            var sub = await uow.SubscriberRepo.FindByTokenAsync(t.Value, isANT: false);
            if (sub == null)
                return View();

            if (!sub.IsActive)
            {
                ViewBag.UnsubResult = "already";
                ViewBag.UnsubEmail = sub.Email;
                return View();
            }

            sub.IsActive = false;
            sub.UnsubscribedTime = DateTime.Now;
            sub.UserLog.UpdateTime = DateTime.Now;
            sub.UserLog.UpdateUserId = "Anonymous";
            await uow.CommitAsync();

            ViewBag.UnsubResult = "done";
            ViewBag.UnsubEmail = sub.Email;
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var dto = new HomePageDto();

            var newestArticles = await uow.ArticleRepo.ActiveArticleANT()
                .OrderByDescending(c => c.OnlineTime).Take(8).ToListAsync();

            dto.NewestArticles = DataGenHelper.GetBaseArticlesFromArticles(newestArticles, uow.ArticleTagRepo, Host, true, false);
            dto.MosaicArticles = dto.NewestArticles.Take(3).ToList();

            var featuredArticles = await uow.ArticleRepo.ActiveArticleANT()
                .Where(c => c.IsFeaturedArticle)
                .OrderBy(c => c.FeaturedArticleDisplaySeq).ThenByDescending(c => c.OnlineTime)
                .Take(6).ToListAsync();
            dto.FeaturedArticles = DataGenHelper.GetBaseArticlesFromArticles(featuredArticles, uow.ArticleTagRepo, Host, true, false);

            dto.ViewArticles = DataGenHelper.GetBaseArticlesFromArticles(
                await uow.ArticleRepo.ActiveArticleANT().OrderByDescending(c => c.Views).Take(8).ToListAsync(),
                uow.ArticleTagRepo, Host, false, true);

            dto.Regions = await GetRegions(20);

            var featuredGuidesRaw = await uow.GuideRepo.QueryActive()
                .OrderByDescending(p => p.IsFeatured).ThenBy(p => p.DisplaySeq)
                .Take(3).ToListAsync();
            dto.FeaturedGuides = featuredGuidesRaw.Select(MapGuideCard).ToList();

            return View(dto);
        }

        private async Task<List<ThemeCategoryDto>> GetThemeCategories(int take)
        {
            return await uow.ArticleCategoryRepo.All()
                .OrderBy(p => p.DisplaySeq)
                .Take(take)
                .Select(p => new ThemeCategoryDto
                {
                    Code = p.Code,
                    Name = p.Name,
                    NameEn = p.NameEn,
                    DisplaySeq = p.DisplaySeq,
                    ArticleCount = p.ArticleCategoryArticles.Count(a => a.Article.ArticleReviewType == ArticleReviewType.已通過 && a.Article.ArticleType == ArticleType.文章)
                })
                .ToListAsync();
        }

        private async Task<List<RegionItemDto>> GetRegions(int take)
        {
            return await uow.RegionRepo.QueryActive()
                .OrderBy(p => p.DisplaySeq)
                .Take(take)
                .Select(p => new RegionItemDto
                {
                    RegionId = p.RegionId,
                    Code = p.Code,
                    Name = p.Name,
                    NameEn = p.NameEn,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    DisplaySeq = p.DisplaySeq,
                    ArticleCount = p.ArticleRegions.Count(a => a.Article.ArticleReviewType == ArticleReviewType.已通過 && a.Article.ArticleType == ArticleType.文章),
                    GuideCount = p.GuideRegions.Count(g => g.Guide.IsActive)
                })
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword = null)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
                return RedirectToAction(nameof(Category), new { s = keyword });

            var dto = new SearchPageDto();

            var hotKeywords = await uow.HotKeywordRepo.All()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplaySeq)
                .Take(6)
                .ToListAsync();
            for (int i = 0; i < hotKeywords.Count; i++)
            {
                dto.HotKeywords.Add(new HotKeywordItemDto
                {
                    Rank = i + 1,
                    Name = hotKeywords[i].Name,
                    Url = StrPath.Search(hotKeywords[i].Name)
                });
            }

            var hotTagsRaw = await uow.TagRepo.All()
                .Where(p => p.ArticleTags.Any(a => a.Article.ArticleReviewType == ArticleReviewType.已通過))
                .Select(p => new { p.Name, Count = p.ArticleTags.Count(a => a.Article.ArticleReviewType == ArticleReviewType.已通過) })
                .OrderByDescending(p => p.Count)
                .Take(10)
                .ToListAsync();
            dto.HotTags = hotTagsRaw.Select(p => new TagCloudItemDto
            {
                Name = p.Name,
                Count = p.Count,
                Url = StrPath.Tag(p.Name)
            }).ToList();

            var picks = await uow.ArticleRepo.ActiveArticleANT()
                .OrderByDescending(p => p.Views)
                .Take(4)
                .ToListAsync();
            dto.EditorPicks = DataGenHelper.GetBaseArticlesFromArticles(picks, uow.ArticleTagRepo, Host, false, true);

            return View(dto);
        }

        public async Task<IActionResult> Category(CategoryListDto dto)
        {
            dto.AllCategories = await GetThemeCategories(20);

            IQueryable<Article> articles = uow.ArticleRepo.ActiveArticleANT();

            if (!string.IsNullOrWhiteSpace(dto.S))
            {
                articles = articles.Where(p => p.Title.Contains(dto.S) || p.Description.Contains(dto.S));
                dto.CurrentName = $"搜尋：{dto.S}";
                dto.CurrentNameEn = "Search Results";
                dto.CurrentArticleCount = await articles.CountAsync();
            }
            else if (!string.IsNullOrWhiteSpace(dto.Id))
            {
                articles = articles.Where(p => p.ArticleCategoryArticles.Any(a => a.ArticleCategory.Code == dto.Id));
                var current = dto.AllCategories.FirstOrDefault(p => p.Code == dto.Id);
                if (current != null)
                {
                    dto.CurrentName = current.Name;
                    dto.CurrentNameEn = current.NameEn;
                    dto.CurrentArticleCount = current.ArticleCount;
                }
            }
            else
            {
                dto.CurrentName = "全部文章";
                dto.CurrentNameEn = "All Journals";
                dto.CurrentArticleCount = await articles.CountAsync();
            }

            // 置頂文章一律排在最上面,其餘依排序條件往下顯示
            articles = dto.SortBy switch
            {
                "views" => articles.OrderByDescending(p => p.IsTop).ThenByDescending(p => p.Views),
                _ => articles.OrderByDescending(p => p.IsTop).ThenByDescending(p => p.OnlineTime),
            };

            dto.TotalArticles = await articles.CountAsync();
            dto.PageSize = 5;
            dto.TotalPages = (int)Math.Ceiling(dto.TotalArticles / (double)dto.PageSize);
            if (dto.PageNo < 1) dto.PageNo = 1;

            var pageList = await articles.Skip((dto.PageNo - 1) * dto.PageSize).Take(dto.PageSize).ToListAsync();
            dto.Articles = DataGenHelper.GetBaseArticlesFromArticles(pageList, uow.ArticleTagRepo, Host, true, true);

            dto.HotTags = await uow.TagRepo.QueryHotTag(dto.Id, null);

            return View(dto);
        }

        public async Task<IActionResult> Region(CategoryListDto dto)
        {
            dto.AllRegions = await GetRegions(20);

            IQueryable<Article> articles = uow.ArticleRepo.ActiveArticleANT();

            if (!string.IsNullOrWhiteSpace(dto.S))
            {
                articles = articles.Where(p => p.Title.Contains(dto.S) || p.Description.Contains(dto.S));
                dto.CurrentName = $"搜尋:{dto.S}";
                dto.CurrentNameEn = "Search Results";
                dto.CurrentArticleCount = await articles.CountAsync();
            }
            else if (!string.IsNullOrWhiteSpace(dto.Id))
            {
                articles = articles.Where(p => p.ArticleRegions.Any(a => a.Region.Code == dto.Id));
                var current = dto.AllRegions.FirstOrDefault(p => p.Code == dto.Id);
                if (current != null)
                {
                    dto.CurrentName = current.Name;
                    dto.CurrentNameEn = current.NameEn;
                    dto.CurrentDescription = current.Description;
                    dto.CurrentArticleCount = current.ArticleCount;
                    dto.CurrentGuideCount = current.GuideCount;
                }
            }
            else
            {
                dto.CurrentName = "全部地區";
                dto.CurrentNameEn = "All Regions";
                dto.CurrentArticleCount = await articles.CountAsync();
            }

            articles = dto.SortBy switch
            {
                "views" => articles.OrderByDescending(p => p.Views),
                _ => articles.OrderByDescending(p => p.IsTop).ThenByDescending(p => p.OnlineTime),
            };

            dto.TotalArticles = await articles.CountAsync();
            dto.PageSize = 5;
            dto.TotalPages = (int)Math.Ceiling(dto.TotalArticles / (double)dto.PageSize);
            if (dto.PageNo < 1) dto.PageNo = 1;

            var pageList = await articles.Skip((dto.PageNo - 1) * dto.PageSize).Take(dto.PageSize).ToListAsync();
            dto.Articles = DataGenHelper.GetBaseArticlesFromArticles(pageList, uow.ArticleTagRepo, Host, true, true);

            dto.HotTags = await uow.TagRepo.QueryHotTag(dto.Id, null);

            return View(dto);
        }

        public async Task<IActionResult> Tag(string id, TagListDto dto)
        {
            if (!string.IsNullOrWhiteSpace(id))
                dto.T = id;

            if (string.IsNullOrWhiteSpace(dto.T))
                return RedirectToAction("Search");

            var allTagsRaw = await uow.TagRepo.All()
                .Where(p => p.ArticleTags.Any(a => a.Article.ArticleReviewType == ArticleReviewType.已通過))
                .Select(p => new { p.Name, p.Description, Count = p.ArticleTags.Count(a => a.Article.ArticleReviewType == ArticleReviewType.已通過) })
                .OrderByDescending(p => p.Count)
                .ToListAsync();

            dto.AllTags = allTagsRaw.Select(p => new TagCloudItemDto
            {
                Name = p.Name,
                Count = p.Count,
                Url = StrPath.Tag(p.Name)
            }).ToList();

            if (!string.IsNullOrWhiteSpace(dto.T))
            {
                var current = allTagsRaw.FirstOrDefault(p => p.Name == dto.T);
                if (current != null)
                {
                    dto.CurrentName = current.Name;
                    dto.CurrentDescription = current.Description;
                    dto.CurrentArticleCount = current.Count;
                }

                IQueryable<Article> articles = uow.ArticleRepo.ActiveArticleANT()
                    .Where(p => p.ArticleTags.Any(a => a.Tag.Name == dto.T));

                articles = dto.SortBy switch
                {
                    "views" => articles.OrderByDescending(p => p.Views),
                    _ => articles.OrderByDescending(p => p.OnlineTime),
                };

                dto.TotalArticles = await articles.CountAsync();
                dto.PageSize = 5;
                dto.TotalPages = (int)Math.Ceiling(dto.TotalArticles / (double)dto.PageSize);
                if (dto.PageNo < 1) dto.PageNo = 1;

                var pageList = await articles.Skip((dto.PageNo - 1) * dto.PageSize).Take(dto.PageSize).ToListAsync();
                dto.Articles = DataGenHelper.GetBaseArticlesFromArticles(pageList, uow.ArticleTagRepo, Host, true, true);

                dto.RelatedTags = await uow.TagRepo.QueryHotTag(null, dto.T);

                var matchedGuides = await uow.GuideRepo.QueryActive()
                    .Where(p => p.GuideTags.Any(gt => gt.Tag.Name == dto.T))
                    .OrderBy(p => p.DisplaySeq)
                    .ToListAsync();
                dto.CurrentGuideCount = matchedGuides.Count;
                dto.Guides = matchedGuides.Take(3).Select(MapGuideCard).ToList();
            }

            return View(dto);
        }

        public async Task<IActionResult> Guides(GuidesPageDto dto)
        {
            var allGuides = await uow.GuideRepo.QueryActive().OrderBy(p => p.DisplaySeq).ToListAsync();

            var regions = await uow.RegionRepo.QueryActive().OrderBy(p => p.DisplaySeq).ToListAsync();
            dto.Cities = regions
                .Select(r => new CityChipDto
                {
                    Name = r.Name,
                    Code = r.Code,
                    Count = allGuides.Count(g => g.GuideRegions != null && g.GuideRegions.Any(gr => gr.RegionId == r.RegionId))
                })
                .Where(c => c.Count > 0)
                .ToList();

            dto.Languages = allGuides
                .Where(p => !string.IsNullOrEmpty(p.Languages))
                .SelectMany(p => p.Languages.Split(new[] { '·', '/', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(l => l.Trim()).Distinct().Take(6).ToList();

            dto.CityCount = dto.Cities.Count;

            var featured = allGuides.FirstOrDefault(p => p.IsFeatured);
            if (featured != null)
                dto.FeaturedGuide = MapGuideCard(featured);

            IEnumerable<Guide> filtered = allGuides;
            if (!string.IsNullOrWhiteSpace(dto.Id))
                filtered = filtered.Where(p => p.GuideRegions != null && p.GuideRegions.Any(gr => gr.Region.Code == dto.Id));

            var totalGuidesList = filtered.ToList();

            dto.TotalGuides = totalGuidesList.Count;
            dto.PageSize = 9;
            dto.TotalPages = (int)Math.Ceiling(dto.TotalGuides / (double)dto.PageSize);
            if (dto.PageNo < 1) dto.PageNo = 1;

            dto.Guides = totalGuidesList
                .Skip((dto.PageNo - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(MapGuideCard).ToList();

            return View(dto);
        }

        public async Task<IActionResult> GuideProfile(string id)
        {
            Guide guide = null;
            if (!string.IsNullOrEmpty(id))
            {
                if (Guid.TryParse(id, out var guideId))
                    guide = await uow.GuideRepo.FindAsync(guideId);
                else
                    guide = await uow.GuideRepo.FindCodeAsync(id);
            }

            if (guide == null)
                guide = await uow.GuideRepo.QueryActive().OrderBy(p => p.DisplaySeq).FirstOrDefaultAsync();

            if (guide == null)
                return View(new GuideProfileDto());

            var guideRegions = guide.GuideRegions?
                .OrderBy(r => r.Region.DisplaySeq)
                .Select(r => new GuideRegionRef { Name = r.Region.Name, Code = r.Region.Code })
                .ToList() ?? new List<GuideRegionRef>();
            var firstRegion = guideRegions.FirstOrDefault();

            var dto = new GuideProfileDto
            {
                GuideId = guide.GuideId,
                Code = guide.GetCodeOrId,
                Name = guide.Name,
                NameEn = guide.NameEn,
                City = firstRegion?.Name,
                CityCode = firstRegion?.Code,
                Regions = guideRegions,
                YearsOfExperience = guide.YearsOfExperience,
                Languages = guide.Languages,
                ServiceFormat = guide.ServiceFormat,
                Bio = guide.Bio,
                LongBio = guide.LongBio,
                QuoteText = guide.QuoteText,
                QuoteBy = guide.QuoteBy,
                PortraitUrl = guide.PortraitUrl,
                CoverUrl = guide.CoverUrl,
                Rating = guide.Rating,
                ReviewCount = guide.ReviewCount,
                MinPeople = guide.MinPeople,
                MaxPeople = guide.MaxPeople,
                ResponseTime = guide.ResponseTime,
                StartingPrice = guide.StartingPrice,
                Tags = guide.GuideTags?.OrderBy(p => p.DisplaySeq).Select(p => p.Tag.Name).ToList() ?? new List<string>(),
                Routes = guide.GuideRoutes?.OrderBy(p => p.DisplaySeq).Select(p => new GuideRouteDto
                {
                    Title = p.Title,
                    Description = p.Description,
                    Duration = p.Duration,
                    Url = p.Url,
                    DisplaySeq = p.DisplaySeq
                }).ToList() ?? new List<GuideRouteDto>(),
                Reviews = guide.GuideReviews?.OrderBy(p => p.DisplaySeq).Take(6).Select(p => new GuideReviewDto
                {
                    ReviewerName = p.ReviewerName,
                    ReviewerFrom = p.ReviewerFrom,
                    Stars = p.Stars,
                    Content = p.Content,
                    TripDate = p.TripDate,
                    TripDays = p.TripDays,
                    City = firstRegion?.Name
                }).ToList() ?? new List<GuideReviewDto>(),
                Faqs = guide.GuideFaqs?.OrderBy(p => p.DisplaySeq).Select(p => new GuideFaqItemDto
                {
                    Question = p.Question,
                    Answer = p.Answer
                }).ToList() ?? new List<GuideFaqItemDto>()
            };

            var articles = guide.Authors?
                .SelectMany(a => a.Articles ?? new List<Article>())
                .Where(p => p.ArticleReviewType == ArticleReviewType.已通過)
                .OrderByDescending(p => p.OnlineTime)
                .ToList() ?? new List<Article>();
            dto.ArticleTotalCount = articles.Count;
            dto.Articles = DataGenHelper.GetBaseArticlesFromArticles(articles.Take(2).ToList(), uow.ArticleTagRepo, Host, true, true);

            var guideRegionIds = guide.GuideRegions?.Select(r => r.RegionId).ToList() ?? new List<Guid>();
            var similar = await uow.GuideRepo.QueryActive()
                .Where(p => p.GuideId != guide.GuideId && p.GuideRegions.Any(gr => guideRegionIds.Contains(gr.RegionId)))
                .OrderBy(p => p.DisplaySeq).Take(3).ToListAsync();
            if (similar.Count < 3)
            {
                var existingIds = similar.Select(s => s.GuideId).ToList();
                var fillCount = 3 - similar.Count;
                var more = await uow.GuideRepo.QueryActive()
                    .Where(p => p.GuideId != guide.GuideId && !existingIds.Contains(p.GuideId))
                    .OrderBy(p => p.DisplaySeq).Take(fillCount).ToListAsync();
                similar.AddRange(more);
            }
            dto.SimilarGuides = similar.Select(MapGuideCard).ToList();

            return View(dto);
        }

        private GuideCardDto MapGuideCard(Guide guide)
        {
            var regions = guide.GuideRegions?
                .OrderBy(r => r.Region.DisplaySeq)
                .Select(r => new GuideRegionRef { Name = r.Region.Name, Code = r.Region.Code })
                .ToList() ?? new List<GuideRegionRef>();
            var firstRegion = regions.FirstOrDefault();
            return new GuideCardDto
            {
                GuideId = guide.GuideId,
                Code = guide.GetCodeOrId,
                Url = $"/GuideProfile/{guide.GetCodeOrId}",
                Name = guide.Name,
                NameEn = guide.NameEn,
                City = firstRegion?.Name,
                CityCode = firstRegion?.Code,
                Regions = regions,
                YearsOfExperience = guide.YearsOfExperience,
                Languages = guide.Languages,
                Bio = guide.Bio,
                PortraitUrl = guide.PortraitUrl,
                CoverUrl = guide.CoverUrl,
                Rating = guide.Rating,
                ReviewCount = guide.ReviewCount,
                ArticleCount = guide.Authors?.SelectMany(a => a.Articles ?? new List<Article>()).Count(p => p.ArticleReviewType == ArticleReviewType.已通過) ?? 0,
                Tags = guide.GuideTags?.OrderBy(p => p.DisplaySeq).Take(3).Select(p => p.Tag.Name).ToList() ?? new List<string>()
            };
        }

        public async Task<IActionResult> Article(string? id = null)
        {
            if (string.IsNullOrEmpty(id))
                return View();

            Article? article = null;

            // Article 觀看計數要改 Views → 用 tracking
            if (Guid.TryParse(id, out Guid articleId))
                article = await uow.ArticleRepo.FindAsync(articleId, isANT: false);
            else
                article = await uow.ArticleRepo.FindCodeAsync(id, isANT: false);

            if (article == null || article.ArticleReviewType != ArticleReviewType.已通過)
            {
                logger.LogInformation("Article not found,id:{Id},ArticleReviewType:{ReviewType}", id, article?.ArticleReviewType);
                return RedirectToAction("NotFound");
            }

            GoRegister = StrPath.ClickUrl(Host, $"{WebConfig.GoRegister}{article.PromotionCode}");

            var dto = await DataGenHelper.GetArticles(uow.ArticleTagRepo, uow.ArticleSDCommonQuestionRepo, article, Host, GoRegister);
            dto.Author = article.Author?.Name;
            dto.AuthorBio = article.Author?.Bio;
            dto.GuideRegions = article.Author?.Guide?.GuideRegions?
                .OrderBy(r => r.Region.DisplaySeq)
                .Select(r => new GuideRegionRef { Name = r.Region.Name, Code = r.Region.Code })
                .ToList() ?? new List<GuideRegionRef>();
            dto.ReadTimeMin = article.ReadTimeMin;
            dto.SerialNumber = article.SerialNumber;
            dto.OnlineTime = article.OnlineTime;
            dto.Views = article.Views;

            var relatedRaw = await uow.ArticleRepo.ActiveArticleANT()
                .Where(p => p.ArticleId != article.ArticleId &&
                    p.ArticleCategoryArticles.Any(a => a.ArticleCategory.Code == dto.CategoryCode))
                .OrderByDescending(p => p.Views)
                .Take(3)
                .ToListAsync();
            dto.RelatedArticles = DataGenHelper.GetBaseArticlesFromArticles(relatedRaw, uow.ArticleTagRepo, Host, false, true);

            var prevRaw = await uow.ArticleRepo.ActiveArticleANT()
                .Where(p => p.OnlineTime < article.OnlineTime)
                .OrderByDescending(p => p.OnlineTime)
                .FirstOrDefaultAsync();
            var nextRaw = await uow.ArticleRepo.ActiveArticleANT()
                .Where(p => p.OnlineTime > article.OnlineTime)
                .OrderBy(p => p.OnlineTime)
                .FirstOrDefaultAsync();
            if (prevRaw != null)
                dto.PrevArticle = DataGenHelper.GetBaseArticlesFromArticles(new List<Article> { prevRaw }, uow.ArticleTagRepo, Host, false, false).First();
            if (nextRaw != null)
                dto.NextArticle = DataGenHelper.GetBaseArticlesFromArticles(new List<Article> { nextRaw }, uow.ArticleTagRepo, Host, false, false).First();

            MetaArticle = article;

            ViewBag.ArticleImagePath = dto.ImagePath;
            ViewBag.ArticleImageMobilePath = dto.ImageMobilePath;

            #region 紀錄Cookie & 統計文章觀看次數

            string cookieName = $"VA-{article.ArticleId}";
            string? cookieValue = Request.Cookies[cookieName];

            if (cookieValue == null)
            {
                var view = new ArticleView
                {
                    ArticleViewId = Guid.NewGuid(),
                    ArticleId = article.ArticleId,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                    CreateTime = DateTime.Now
                };
                uow.ArticleViewRepo.Add(view);

                article.Views++;
                await uow.CommitAsync();

                Response.Cookies.Append(cookieName, view.ArticleViewId.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(1)
                });
            }

            #endregion

            return View(dto);
        }

        public async Task<IActionResult> FAQ()
        {
            var vm = new FaqDto();
            var questionAnswers = await uow.QuestionAnswerRepo.All().OrderBy(p => p.DisplaySeq).ToListAsync();
            for (int i = 0; i < questionAnswers.Count; i++)
            {
                var questionAnswerDto = new QuestionAnswerDto();
                vm.QuestionAnswers.Add(questionAnswerDto);
                questionAnswerDto.Title = questionAnswers[i].Title;
                questionAnswerDto.Content = questionAnswers[i].Content;
                questionAnswerDto.DisplaySeq = questionAnswers[i].DisplaySeq;
                questionAnswerDto.Url = questionAnswers[i].Url;
                questionAnswerDto.ShowNumber = $"Q{i + 1}";
            }

            return View(vm);
        }

        public IActionResult Privacy() => View();

        public IActionResult Terms() => View();

        private async Task<AsideSearchTagDto> GetAsideSearchTagDto(string? code = null, string? tag = null)
        {
            var dto = new AsideSearchTagDto();
            dto.HotTags = await uow.TagRepo.QueryHotTag(code, tag);
            return dto;
        }

        public async Task<IActionResult> SitemapXml()
        {
            var host = $"{Request.Scheme}://{Request.Host}";
            var articleCategoryCodeList = await uow.ArticleCategoryRepo.All()
                .OrderBy(p => p.DisplaySeq).Select(p => p.Code).ToListAsync();
            var articles = await uow.ArticleRepo.IncludeAll()
                .Where(p => p.ArticleReviewType == ArticleReviewType.已通過)
                .OrderBy(p => p.DisplaySeq).ToListAsync();
            var articleIdList = articles.Select(p => p.GetCodeOrId).ToList();
            var articleOnlineTimeList = new List<DateTime>();
            foreach (var article in articles)
            {
                if (article.UserLog.UpdateTime > article.OnlineTime)
                    articleOnlineTimeList.Add(article.UserLog.UpdateTime!.Value);
                else
                    articleOnlineTimeList.Add(article.OnlineTime!.Value);
            }

            var articleTags = await uow.ArticleTagRepo.IncludeAll()
                .Where(p => p.Article.ArticleReviewType == ArticleReviewType.已通過).ToListAsync();
            var tempTagList = new Dictionary<string, int>();
            foreach (var item in articleTags)
            {
                if (!tempTagList.ContainsKey(item.Tag.Name))
                    tempTagList[item.Tag.Name] = item.Tag.DisplaySeq;
            }
            var tagList = tempTagList.OrderBy(p => p.Value).Select(p => p.Key).ToList();

            var guides = await uow.GuideRepo.QueryActive()
                .OrderBy(p => p.DisplaySeq)
                .Select(p => new { p.Code, p.GuideId, p.UserLog.UpdateTime, p.UserLog.CreateTime })
                .ToListAsync();
            var guideList = guides.Select(g => (
                Code: !string.IsNullOrWhiteSpace(g.Code) ? g.Code : g.GuideId.ToString(),
                LastMod: (DateTime?)(g.UpdateTime ?? g.CreateTime)
            )).ToList();

            var sitemapNodes = SitemapHelper.GetSitemapNodes(host, articleCategoryCodeList, articleIdList, articleOnlineTimeList, tagList, guideList);
            string xml = SitemapHelper.GetSitemapDocument(sitemapNodes);
            Response.Headers["Cache-Control"] = "public, max-age=3600";
            return Content(xml, "application/xml", Encoding.UTF8);
        }
    }
}
