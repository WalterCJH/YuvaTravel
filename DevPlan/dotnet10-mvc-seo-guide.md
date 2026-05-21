# .NET 10 MVC 網站 SEO 開發指引

> 本文件供 Claude Code 在開發 ASP.NET Core MVC (.NET 10) 網站時參考。
> 內容整合 Google 官方 SEO 入門指南 (2025-12-18 最新版) 與 .NET 10 / ASP.NET Core 10 (LTS, 2025-11-11 發布) 的實務做法。
>
> 參考來源：
> - https://developers.google.com/search/docs/fundamentals/seo-starter-guide?hl=zh-tw
> - https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-10.0
> - Google March 2026 Core Update 內容品質方向

---

## 一、Google SEO 核心原則 (2026 最新方向)

Google 在 2026 年 3 月 Core Update 後，排名機制更強調以下三個維度，開發網站時必須讓**內容、結構、技術**三者同時對齊。

### 1. 內容原創性 (Information Originality)
- 不是「不能用 AI 寫」，而是「內容必須有別處沒有的東西」。
- AI 草稿 + 真人專家編修是可接受的；單純 AI 量產則會被降權。
- 每一頁都應該回答「使用者來這裡能得到什麼別人沒有的價值？」。

### 2. 作者專業度 (Author Expertise / E-E-A-T)
- E-E-A-T = Experience（經驗）、Expertise（專業）、Authoritativeness（權威）、Trustworthiness（信任）。
- 文章應有可驗證的作者資訊；匿名或假帳號內容會掉名次。
- YMYL（健康、財務、法律等）類型網站更必須標明作者背景。

### 3. 主題一致性 (Topical Coherence)
- 網站如果聚焦在一個領域，整體權重會更強。
- 網域內把一個主題寫深寫滿，比廣泛但淺薄的內容更被青睞。
- 開發前先想清楚：**這個網站是誰的、為了什麼主題而存在**。

### 其他不會變的基本原則
- 內容必須以使用者為優先（People-First Content）。
- 不要為了排名而做標題、URL、關鍵字堆疊。
- 只用 robots.txt / noindex 阻擋不該被索引的頁面（後台、測試頁、個人草稿）。
- 任何變更都需要時間反映在搜尋結果，通常 **數週至數月**。

---

## 二、.NET 10 MVC 專案 SEO 實作清單

### 2.1 專案設定

#### 啟用 HTTPS（必要）
Google 對 HTTP 網站給予較低信任。`Program.cs` 中：

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// 強制 HTTPS
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443;
});

// HSTS（生產環境）
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
```

#### 區分環境，避免測試站被索引
正式站才允許爬蟲索引；Staging / Dev 一律禁止。
使用 `app.Environment.IsProduction()` 來切換 robots.txt 行為。

---

### 2.2 URL 結構

Google 偏好「看得懂、人類可讀」的 URL。

#### 好的 URL
```
https://example.com/products/cat-food
https://example.com/blog/2026/seo-checklist
```

#### 不好的 URL
```
https://example.com/Product?id=12345
https://example.com/p/2/6772756D707920636174
```

#### MVC 路由建議
- 全部用**小寫**、用**連字號 `-`**（不要底線 `_`）。
- 路徑包含關鍵字，避免純 ID。
- 同時保留 ID 確保唯一性：`/products/{id}/{slug}`。

```csharp
// Program.cs - 強制小寫 URL
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
    options.AppendTrailingSlash = false;
});

// 自訂路由示例
app.MapControllerRoute(
    name: "product",
    pattern: "products/{id:int}/{slug?}",
    defaults: new { controller = "Products", action = "Detail" });
```

#### Slug 產生（Helper）
```csharp
public static class SlugHelper
{
    public static string ToSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var normalized = input.ToLowerInvariant().Trim();
        normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"[^a-z0-9\u4e00-\u9fa5\s-]", "");
        normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"\s+", "-");
        return normalized;
    }
}
```

> 中文站可保留中文 slug，Google 支援 UTF-8 URL；但若是 B2B / 國際站，建議用英文 slug。

---

### 2.3 Meta Tag（每一頁必備）

#### Layout 中加入動態 Meta
`_Layout.cshtml`：

```html
<!DOCTYPE html>
<html lang="@(ViewData["Lang"] ?? "zh-Hant")">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>@(ViewData["Title"] ?? "預設網站名稱")</title>
    <meta name="description" content="@(ViewData["Description"] ?? "")" />

    @* Canonical *@
    <link rel="canonical" href="@(ViewData["Canonical"] ?? Context.Request.GetDisplayUrl())" />

    @* Open Graph (Facebook / LINE / 一般社群) *@
    <meta property="og:title" content="@(ViewData["OgTitle"] ?? ViewData["Title"])" />
    <meta property="og:description" content="@(ViewData["OgDescription"] ?? ViewData["Description"])" />
    <meta property="og:image" content="@(ViewData["OgImage"] ?? "/images/og-default.jpg")" />
    <meta property="og:url" content="@(ViewData["Canonical"] ?? Context.Request.GetDisplayUrl())" />
    <meta property="og:type" content="@(ViewData["OgType"] ?? "website")" />
    <meta property="og:locale" content="zh_TW" />

    @* Twitter Card *@
    <meta name="twitter:card" content="summary_large_image" />

    @* 索引控制（預設可索引）*@
    @if (ViewData["NoIndex"] is true)
    {
        <meta name="robots" content="noindex, nofollow" />
    }
    else
    {
        <meta name="robots" content="index, follow" />
    }

    @await RenderSectionAsync("Head", required: false)
</head>
```

#### 各頁設定方式
```csharp
// 在 Action 中
public IActionResult Detail(int id)
{
    var product = _service.Get(id);
    ViewData["Title"] = $"{product.Name} | 沐澄科技";
    ViewData["Description"] = product.ShortDescription; // 控制在 120-160 字內
    ViewData["Canonical"] = Url.Action("Detail", "Products", new { id, slug = product.Slug }, Request.Scheme);
    ViewData["OgImage"] = product.MainImageUrl;
    return View(product);
}
```

#### Meta 規則
- `<title>`：建議 30–60 字元（中文約 15–30 字）。
- `<meta name="description">`：120–160 字元，每頁不同，要含關鍵字但不堆疊。
- 後台 / 會員中心 / 搜尋結果頁 → 設 `noindex`。

---

### 2.4 結構化資料 (Schema.org / JSON-LD)

JSON-LD 是 Google 最推薦的格式。可拿到精選摘要、複合式搜尋結果。

#### 範例：機構資訊 (Organization)
```html
<script type="application/ld+json">
{
  "@@context": "https://schema.org",
  "@@type": "Organization",
  "name": "沐澄科技",
  "url": "https://example.com",
  "logo": "https://example.com/logo.png",
  "sameAs": [
    "https://www.facebook.com/example",
    "https://www.linkedin.com/company/example"
  ]
}
</script>
```

> 注意：Razor 中要寫 `@@` 來輸出 `@` 字元。

#### 範例：文章 (Article)
```csharp
// 建立 Tag Helper 或 ViewComponent 統一輸出
@{
    var article = Model;
    var jsonLd = System.Text.Json.JsonSerializer.Serialize(new
    {
        context = "https://schema.org",
        type = "Article",
        headline = article.Title,
        author = new { type = "Person", name = article.AuthorName },
        datePublished = article.PublishedAt.ToString("yyyy-MM-dd"),
        image = article.CoverImageUrl
    });
}
<script type="application/ld+json">@Html.Raw(jsonLd)</script>
```

#### 常用類型
- `Organization` / `LocalBusiness`：所有網站都建議放
- `Article` / `BlogPosting`：部落格、新聞
- `Product` + `Offer` + `AggregateRating`：電商產品頁
- `BreadcrumbList`：麵包屑導覽
- `FAQPage`：常見問題（注意 Google 已縮減此類型在搜尋結果的顯示）
- `Event`：活動、演講
- `JobPosting`：徵才頁

#### 驗證工具
- [Google Rich Results Test](https://search.google.com/test/rich-results)
- [Schema Markup Validator](https://validator.schema.org/)

---

### 2.5 Sitemap.xml 動態產生

使用 MVC Action 動態產生，比靜態檔案易維護。

#### 簡單實作
```csharp
[Route("sitemap.xml")]
public class SitemapController : Controller
{
    private readonly IProductService _products;
    private readonly IArticleService _articles;

    public SitemapController(IProductService products, IArticleService articles)
    {
        _products = products;
        _articles = articles;
    }

    [ResponseCache(Duration = 3600)] // 快取 1 小時
    public async Task<IActionResult> Index()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var urls = new List<SitemapUrl>
        {
            new() { Loc = baseUrl, ChangeFreq = "daily", Priority = 1.0M, LastMod = DateTime.UtcNow },
            new() { Loc = $"{baseUrl}/about", ChangeFreq = "monthly", Priority = 0.6M },
        };

        // 動態加入產品
        var products = await _products.GetActiveAsync();
        urls.AddRange(products.Select(p => new SitemapUrl
        {
            Loc = $"{baseUrl}/products/{p.Id}/{p.Slug}",
            LastMod = p.UpdatedAt,
            ChangeFreq = "weekly",
            Priority = 0.8M
        }));

        // 動態加入文章
        var articles = await _articles.GetPublishedAsync();
        urls.AddRange(articles.Select(a => new SitemapUrl
        {
            Loc = $"{baseUrl}/blog/{a.Slug}",
            LastMod = a.UpdatedAt,
            ChangeFreq = "monthly",
            Priority = 0.7M
        }));

        var xml = BuildSitemapXml(urls);
        return Content(xml, "application/xml; charset=utf-8");
    }

    private static string BuildSitemapXml(IEnumerable<SitemapUrl> urls)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        sb.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");
        foreach (var u in urls)
        {
            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{System.Net.WebUtility.HtmlEncode(u.Loc)}</loc>");
            if (u.LastMod.HasValue)
                sb.AppendLine($"    <lastmod>{u.LastMod.Value:yyyy-MM-ddTHH:mm:sszzz}</lastmod>");
            if (!string.IsNullOrEmpty(u.ChangeFreq))
                sb.AppendLine($"    <changefreq>{u.ChangeFreq}</changefreq>");
            sb.AppendLine($"    <priority>{u.Priority:0.0}</priority>");
            sb.AppendLine("  </url>");
        }
        sb.AppendLine("</urlset>");
        return sb.ToString();
    }

    public record SitemapUrl
    {
        public string Loc { get; init; } = "";
        public DateTime? LastMod { get; init; }
        public string? ChangeFreq { get; init; }
        public decimal Priority { get; init; } = 0.5M;
    }
}
```

#### Sitemap 規則
- URL 數量超過 50,000 或檔案超過 50MB → 拆分多個 sitemap，用 sitemap index 串連。
- 不要把 `noindex` 的頁面放進 sitemap。
- 不要把 4xx、5xx、重新導向的頁面放進 sitemap。
- `<lastmod>` 要忠實反映內容更新時間，不要每次都用今天。

---

### 2.6 Robots.txt 動態產生

```csharp
[Route("robots.txt")]
public class RobotsController : Controller
{
    private readonly IWebHostEnvironment _env;

    public RobotsController(IWebHostEnvironment env) => _env = env;

    public IActionResult Index()
    {
        var sb = new System.Text.StringBuilder();

        if (_env.IsProduction())
        {
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow: /admin/");
            sb.AppendLine("Disallow: /api/");
            sb.AppendLine("Disallow: /account/");
            sb.AppendLine("Disallow: /search?");
            sb.AppendLine();
            sb.AppendLine($"Sitemap: {Request.Scheme}://{Request.Host}/sitemap.xml");
        }
        else
        {
            // 開發 / Staging：完全禁止索引
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow: /");
        }

        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }
}
```

---

### 2.7 重新導向與標準化 (Canonical)

#### 301 永久重新導向
網站搬遷、URL 改版時務必用 **301**（不是 302）。

```csharp
// 舊網址導到新網址
app.MapGet("/old-page", () => Results.RedirectToRoute("ProductDetail",
    new { id = 1, slug = "new-slug" }, permanent: true));
```

#### 統一網域
強制 `www` 或不強制，但二選一不要兩個都通：

```csharp
app.Use(async (context, next) =>
{
    var host = context.Request.Host.Host;
    if (host.StartsWith("www."))
    {
        var newUrl = $"{context.Request.Scheme}://{host[4..]}{context.Request.Path}{context.Request.QueryString}";
        context.Response.Redirect(newUrl, permanent: true);
        return;
    }
    await next();
});
```

#### Canonical Tag
分頁、篩選、追蹤參數的頁面，務必指定主要 URL：
```html
<link rel="canonical" href="https://example.com/products/123/cat-food" />
```

---

### 2.8 圖片 SEO

```html
<img src="/images/products/cat-food-large.webp"
     alt="天然成貓主食罐 - 鮭魚口味"
     width="800" height="600"
     loading="lazy"
     decoding="async" />
```

#### 檢查清單
- 一定要有 `alt`，描述圖片內容（不要只寫 `image1.jpg`）。
- 用 WebP / AVIF 格式減少體積。
- 加 `width` / `height` 避免版面跳動（影響 CLS）。
- 非首屏圖片加 `loading="lazy"`。
- 檔名用語意命名：`cat-food-salmon.webp` 比 `IMG_2034.jpg` 好。

---

### 2.9 Core Web Vitals 與效能

Google 將效能列為排名訊號之一，三個指標：
- **LCP** (Largest Contentful Paint) ≤ 2.5s
- **INP** (Interaction to Next Paint) ≤ 200ms
- **CLS** (Cumulative Layout Shift) ≤ 0.1

#### .NET 10 MVC 效能設定

```csharp
// Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

builder.Services.AddResponseCaching();
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(b => b.Expire(TimeSpan.FromMinutes(5)));
    options.AddPolicy("Articles", b => b
        .Tag("articles")
        .Expire(TimeSpan.FromMinutes(30)));
});

var app = builder.Build();

app.UseResponseCompression();
app.UseResponseCaching();
app.UseOutputCache();

// 靜態檔加長快取
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24 * 30; // 30 天
        ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={durationInSeconds}");
    }
});
```

#### 前端最佳化
- CSS / JS 經 bundling + minification（用 webpack、Vite 或 BundlerMinifier）。
- 關鍵 CSS inline 在 `<head>`。
- 字型用 `font-display: swap` 避免 FOIT。
- 預載重要資源：`<link rel="preload" as="font" ...>`。

---

### 2.10 行動版友善 (Mobile-First Indexing)

Google 已是 **行動版優先索引**，行動體驗等於 SEO 體驗。

#### 必備
```html
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
```
- 採用響應式設計（不要做獨立 m. 子網域）。
- 點擊目標 ≥ 48×48px。
- 字體 ≥ 16px。
- 測試工具：[Google Mobile-Friendly Test](https://search.google.com/test/mobile-friendly)。

---

### 2.11 國際化 / 多語系

如果網站有多語版，加入 `hreflang`：

```html
<link rel="alternate" hreflang="zh-Hant" href="https://example.com/zh-tw/" />
<link rel="alternate" hreflang="zh-Hans" href="https://example.com/zh-cn/" />
<link rel="alternate" hreflang="en" href="https://example.com/en/" />
<link rel="alternate" hreflang="x-default" href="https://example.com/" />
```

#### MVC 路由設計
```csharp
app.MapControllerRoute(
    name: "localized",
    pattern: "{culture=zh-tw}/{controller=Home}/{action=Index}/{id?}",
    constraints: new { culture = "zh-tw|zh-cn|en" });
```

---

### 2.12 安全性 Header（間接影響 SEO）

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    // CSP 視專案需求調整
    await next();
});
```

---

### 2.13 自訂錯誤頁與正確 HTTP 狀態碼

爬蟲對錯誤頁只看一件事：**HTTP 狀態碼是否誠實反映頁面狀態**。畫面好不好看 Google 不在乎。

#### 三大原則

1. **狀態碼誠實**：404 頁必須回 404、500 頁必須回 500，**絕對不能回 200**。
2. **使用 `ReExecute` 而非 `Redirect`**：保留使用者原始網址，狀態碼也不會掉。
3. **錯誤頁本身加 `noindex`**：避免 `/Error/404` 這類路徑被搜尋引擎收錄。

#### ❌ 常見錯誤設計

```
https://yourdomain/abc           → 404 頁，但回 HTTP 200 ❌
https://yourdomain/Error/403     → 直接可訪問，回 200 ❌
https://yourdomain/Error/500     → 直接可訪問，回 200 ❌
```

這種設計的後果：
- Google 把錯誤頁當正常頁索引，搜尋結果可能出現「伺服器發生錯誤」字樣。
- 不存在的網址被視為 200，整個網站會被塞進大量空白索引。
- 索引涵蓋率報表會出現「軟性 404」警告。

#### ✅ 正確設計

```csharp
// Program.cs
// 用 ReExecute（不是 Redirect）—— 內部換內容、保留原始 URL 與狀態碼
app.UseStatusCodePagesWithReExecute("/Error/{0}");
```

```csharp
// ErrorController.cs
public class ErrorController : Controller
{
    [Route("Error/{statusCode:int}")]
    public IActionResult Index(int statusCode)
    {
        // 判斷是否為內部 ReExecute 轉進來（而非使用者直接打網址）
        var reExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
        if (reExecuteFeature is null)
        {
            // 使用者直接訪問 /Error/500 → 視為不存在的網址
            Response.StatusCode = 404;
            Response.Headers["X-Robots-Tag"] = "noindex, nofollow";
            return View("NotFound");
        }

        // 正常情況：誠實回傳狀態碼
        Response.StatusCode = statusCode;
        Response.Headers["X-Robots-Tag"] = "noindex, nofollow";

        return statusCode switch
        {
            404 => View("NotFound"),
            403 => View("Forbidden"),
            500 => View("ServerError"),
            _   => View("GenericError")
        };
    }
}
```

#### 雙重保險：robots.txt 也擋掉 `/Error/`

```
User-agent: *
Disallow: /Error/
```

#### 預期行為

| 使用者輸入 | 瀏覽器網址列 | HTTP 狀態碼 | 顯示內容 |
|---|---|---|---|
| `/abc`（不存在的網址） | `/abc` ✅ | 404 ✅ | 自訂 404 頁 |
| 觸發伺服器錯誤的操作 | 原始 URL ✅ | 500 ✅ | 自訂 500 頁 |
| 沒權限的頁面 | 原始 URL ✅ | 403 ✅ | 自訂 403 頁 |
| 直接訪問 `/Error/500` | `/Error/500` | 404 ✅ | 自訂 404 頁 |

> **為什麼用 `ReExecute` 而非 `Redirect`？**
> - `Redirect` 會送 302 跳轉到錯誤頁網址，原本的 404/500 狀態碼會被「沖掉」變成 200。
> - `ReExecute` 是伺服器內部換內容，使用者網址列保持原 URL，狀態碼保持原樣。SEO 與使用者體驗都比較好（可重新整理、可複製網址回報）。

#### 驗證

實作完用 curl 確認每個狀態碼都對：

```bash
curl -I https://yourdomain/abc                  # 期待：HTTP/2 404
curl -I https://yourdomain/page-that-throws     # 期待：HTTP/2 500
curl -I https://yourdomain/forbidden-page       # 期待：HTTP/2 403
curl -I https://yourdomain/Error/500            # 期待：HTTP/2 404（防止被索引）
curl -I https://yourdomain/                     # 期待：HTTP/2 200
```

也可以用 Google Search Console 的「網址檢查工具」確認 Google 認定的狀態碼。

> **重要**：自訂 404 頁回傳 HTTP 200 → Google 誤以為頁面正常 → 索引大量空白頁 → 嚴重傷害 SEO。這是新手最常踩的雷。

---

### 2.14 內容渲染：MVC vs Blazor 的 SEO 取捨

- **傳統 MVC (Razor Server-Side Rendering)** → SEO 友善，HTML 直接由伺服器產生。
- **Blazor Server** → SEO 表現尚可，第一次 HTML 仍由伺服器送出。
- **Blazor WebAssembly** → SEO 較差，內容靠 JS 在客戶端渲染；如要做 SEO 重的網站，**避免**或搭配伺服器端預渲染。
- 重要的內容必須在 **HTML 原始碼** 就出現，不能只靠 JavaScript。

---

## 三、開發流程檢查清單

### 開發階段
- [ ] 路由全部小寫、用連字號。
- [ ] 每頁有獨立的 `<title>` 與 `<meta description>`。
- [ ] 每頁有 `<link rel="canonical">`。
- [ ] 圖片有 `alt`、`width`、`height`。
- [ ] 表單頁、後台、API → 設 `noindex`。
- [ ] 自訂 404 / 403 / 500 頁回傳真正的對應狀態碼（用 curl -I 驗證）。
- [ ] 錯誤頁加 `X-Robots-Tag: noindex` 並在 robots.txt 擋 `/Error/`。
- [ ] 使用 `UseStatusCodePagesWithReExecute`（非 Redirect）保留原始 URL。
- [ ] HTTPS 強制重新導向。
- [ ] 結構化資料用 Rich Results Test 驗證通過。
- [ ] 響應式設計、行動版可用。

### 上線前
- [ ] `sitemap.xml` 可讀且可被瀏覽器開啟。
- [ ] `robots.txt` 行為正確（正式站允許、非正式站禁止）。
- [ ] 生產環境 HSTS 已開。
- [ ] Open Graph 圖片可在 Facebook / LINE 預覽（用 [Sharing Debugger](https://developers.facebook.com/tools/debug/)）。
- [ ] PageSpeed Insights 行動版分數 ≥ 70。
- [ ] 所有舊 URL 都有 301 對應到新 URL（如為改版）。

### 上線後
- [ ] 在 [Google Search Console](https://search.google.com/search-console) 註冊網站。
- [ ] 提交 sitemap.xml。
- [ ] 安裝 Google Analytics（GA4）。
- [ ] 監控 Core Web Vitals 報表。
- [ ] 上線兩週後檢查索引涵蓋範圍報表。
- [ ] 設定 Bing Webmaster Tools。

---

## 四、.NET 10 SEO 相關 NuGet 套件參考

| 套件 | 用途 |
|---|---|
| `SimpleMvcSitemap` | 產生 sitemap.xml |
| `Winton.AspNetCore.Seo` | sitemap + robots.txt + Open Graph 整合 |
| `WebMarkupMin.AspNetCore` | HTML / CSS / JS 壓縮 |
| `NWebsec.AspNetCore.Middleware` | 安全性 Header |
| `Schema.NET` | 強型別產生 Schema.org JSON-LD |

> 自己手寫 sitemap / robots controller 也完全可以，套件不是必要。

---

## 五、Google Search Console 必看報表

1. **索引涵蓋範圍**：哪些頁有被索引、為何沒被索引。
2. **網頁體驗 / Core Web Vitals**：LCP、INP、CLS 表現。
3. **效能 → 搜尋結果**：點擊率、曝光、平均排名、實際搜尋字。
4. **複合式搜尋結果**：結構化資料是否被認列。
5. **連結報表**：誰連到你的網站、最受歡迎的連結文字。

---

## 六、常見錯誤（避雷）

| 錯誤 | 後果 |
|---|---|
| 把整個 `<body>` 用 JS 動態載入 | 內容未被索引 |
| 自訂 404 頁回傳 200 | 索引大量空白頁、出現「軟性 404」警告 |
| `/Error/500` 可直接訪問且回 200 | 錯誤頁被索引，搜尋結果出現「伺服器錯誤」 |
| 用 `Redirect`（而非 `ReExecute`）導向錯誤頁 | 狀態碼變 302/200，搜尋引擎誤判 |
| 同一內容多個 URL（`/?utm=...`、`/page/`、`/Page`）沒設 canonical | 內容稀釋 |
| sitemap.xml 包含 noindex 頁 | Search Console 報錯 |
| 阻擋 `*.css` `*.js` 不讓爬蟲讀 | Google 看到的版面與使用者不同，可能降權 |
| 改版網址沒做 301 | 舊排名歸零 |
| Title / Description 全網站一樣 | 摘要被 Google 重寫，CTR 低 |
| 圖片用 `alt=""` 或缺 alt | 圖片搜尋拿不到流量 |

---

## 七、給 Claude Code 的開發指令模板

寫程式時可以直接這樣下指令：

> 「請幫我在這個 .NET 10 MVC 專案中加入 SEO 基本配置，包含：
> 1. 動態產生的 sitemap.xml controller，從 ProductService 與 ArticleService 取資料；
> 2. 環境感知的 robots.txt，正式站開放、其他環境禁止；
> 3. `_Layout.cshtml` 加入動態 title / description / canonical / Open Graph；
> 4. 強制小寫 URL 與 HTTPS 重新導向；
> 5. 自訂 404 頁要回傳真正的 404 狀態碼。
> 程式風格參考本指引第二節。」

---

**最後提醒**：SEO 沒有捷徑。技術做對只是門票，內容品質、原創性與作者權威才是 2026 年後排名的決定性因素。網站架完之後，把心力放在「寫別人沒寫過、寫得比別人深」的內容上，比反覆調整 meta tag 有用得多。
