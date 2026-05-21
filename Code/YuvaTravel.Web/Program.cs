using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using YuvaTravel.Data;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // === 隱藏 Kestrel Server header ===
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.AddServerHeader = false;
    });

    // NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // === SEO: 強制小寫 URL,canonical 一致 ===
    builder.Services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
        options.LowercaseQueryStrings = false; // 查詢字串保留原樣 (搜尋關鍵字大小寫敏感)
        options.AppendTrailingSlash = false;
    });

    // === SEO + 效能: Response Compression (Brotli + Gzip) ===
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
        {
            "image/svg+xml",
            "application/manifest+json",
            "application/ld+json"
        });
    });
    builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = System.IO.Compression.CompressionLevel.Optimal);
    builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = System.IO.Compression.CompressionLevel.Optimal);

    // === HSTS (僅在 HTTPS) ===
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });

    // === HTTPS 強制重新導向 ===
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    });

    // === Anti-CSRF ===
    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-XSRF-TOKEN";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

    // === Cookie 全域政策 ===
    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        options.MinimumSameSitePolicy = SameSiteMode.Lax;
        options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
        options.Secure = CookieSecurePolicy.SameAsRequest;
    });

    // Session
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(480);
        options.Cookie.Name = ".YT.s";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

    // Cookie Authentication
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/Admin/Account/Login";
            options.AccessDeniedPath = "/Admin/Account/Login";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(480);
            options.SlidingExpiration = true;
            options.Cookie.Name = ".YT.a";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Lax;
        })
        .AddGoogle(options =>
        {
            options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
            options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
        });

    builder.Services.AddControllersWithViews()
        .AddNewtonsoftJson(opts =>
        {
            // .NET 10 起 AddNewtonsoftJson 預設改成 CamelCaseNamingStrategy,
            // 會把 `IsSuccess` 序列化成 `isSuccess`,造成既有 JS 用 PascalCase 讀屬性壞掉。
            // 改回 DefaultContractResolver (不指定 NamingStrategy) 保持原樣 PascalCase。
            opts.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
        });

    var connectionString = builder.Configuration.GetConnectionString("YuvaTravelConnection");
    builder.Services.AddDbContext<YuvaTravelDbContext>(options =>
        options.UseSqlServer(connectionString));

    // === UoW + Repository DI ===
    builder.Services.AddScoped<YuvaTravel.Data.Uow.IUnitOfWork, YuvaTravel.Data.Uow.EFUnitOfWork>();

    // === 文章上線排程背景服務 (.NET 內建 BackgroundService) ===
    builder.Services.AddHostedService<YuvaTravel.Web.BackgroundServices.ScheduledArticlePublisher>();

    builder.Services.AddHttpContextAccessor();

    // === Email (SMTP) 設定,從 appsettings 的 "Email" 區塊綁定 ===
    var emailOptions = new YuvaTravel.Infrastructure.Model.Mail.EmailOptions();
    builder.Configuration.GetSection("Email").Bind(emailOptions);
    builder.Services.AddSingleton(emailOptions);

    // === Cloudflare R2 / 本機 圖片上傳服務 ===
    var r2Options = new YuvaTravel.Infrastructure.Storage.R2Options();
    builder.Configuration.GetSection("Storage:R2").Bind(r2Options);
    builder.Services.AddSingleton(r2Options);
    builder.Services.AddScoped<YuvaTravel.Infrastructure.Storage.IImageUploadService>(sp =>
    {
        var opts = sp.GetRequiredService<YuvaTravel.Infrastructure.Storage.R2Options>();
        if (opts.Enabled && !string.IsNullOrWhiteSpace(opts.AccessKeyId))
        {
            return new YuvaTravel.Infrastructure.Storage.R2ImageUploadService(
                opts,
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<YuvaTravel.Infrastructure.Storage.R2ImageUploadService>>());
        }
        return new YuvaTravel.Infrastructure.Storage.LocalImageUploadService(
            sp.GetRequiredService<IWebHostEnvironment>());
    });

    var app = builder.Build();

    // === 全域安全標頭 (放在最前面) ===
    app.Use(async (context, next) =>
    {
        var headers = context.Response.Headers;

        // 移除暴露技術的 header
        headers.Remove("Server");
        headers.Remove("X-Powered-By");
        headers.Remove("X-AspNet-Version");
        headers.Remove("X-AspNetMvc-Version");

        // 安全標頭
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "SAMEORIGIN";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=(), usb=()";
        headers["X-XSS-Protection"] = "0"; // 現代瀏覽器已棄用,顯式關閉避免被舊版本誤判
        headers["Cross-Origin-Opener-Policy"] = "same-origin";
        headers["Cross-Origin-Resource-Policy"] = "same-site";

        // === SEO: 不該被索引的路徑加 X-Robots-Tag ===
        var p = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (p.StartsWith("/admin") || p.StartsWith("/account")
            || p.StartsWith("/error") || p.StartsWith("/search")
            || p.StartsWith("/lazyload"))
        {
            headers["X-Robots-Tag"] = "noindex, nofollow";
        }

        // CSP - 因為網站使用 inline scripts 和外部 CDN,採用較寬鬆但有保護的策略
        headers["Content-Security-Policy"] =
            "default-src 'self' https: data:; " +
            "script-src 'self' 'unsafe-inline' https:; " +
            "style-src 'self' 'unsafe-inline' https:; " +
            "img-src 'self' https: data: blob:; " +
            "font-src 'self' https: data:; " +
            "connect-src 'self' https:; " +
            "frame-ancestors 'self'; " +
            "base-uri 'self'; " +
            "object-src 'none'; " +
            "form-action 'self';";

        await next();
    });

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error/500");
        app.UseHsts();
    }

    // 404 / 其他狀態碼導向 Error 頁
    app.UseStatusCodePagesWithReExecute("/Error/{0}");

    app.UseHttpsRedirection();

    // === Response Compression (放在 StaticFiles 之前才會壓縮靜態檔) ===
    app.UseResponseCompression();

    // 禁止存取敏感檔案 (.config, .cs, .csproj, .json 等)
    app.Use(async (context, next) =>
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        string[] blocked = { ".config", ".cs", ".csproj", ".cshtml", ".sln", ".user", ".pdb", ".db", ".mdf" };
        if (blocked.Any(ext => path.EndsWith(ext)))
        {
            context.Response.StatusCode = 404;
            return;
        }
        await next();
    });

    app.UseStaticFiles(new Microsoft.AspNetCore.Builder.StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            // 靜態資源安全 header + 長快取 (30 天)
            ctx.Context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            const int durationInSeconds = 60 * 60 * 24 * 30;
            ctx.Context.Response.Headers["Cache-Control"] = $"public,max-age={durationInSeconds}";
        }
    });

    app.UseRouting();

    app.UseCookiePolicy();
    app.UseSession();

    app.UseAuthentication();
    app.UseAuthorization();

    // Admin Area
    app.MapAreaControllerRoute(
        name: "Admin_default",
        areaName: "Admin",
        pattern: "Admin/{controller=Dashbroad}/{action=Index}/{id?}");

    // SiteMap
    app.MapControllerRoute(
        name: "SiteMap",
        pattern: "sitemap.xml",
        defaults: new { controller = "Home", action = "SitemapXml" });

    // LazyLoad
    app.MapControllerRoute(
        name: "LazyLoad",
        pattern: "LazyLoad/{action}/{id?}",
        defaults: new { controller = "LazyLoad", action = "Index" });

    // DefaultHome - 單段 URL 走 Home Controller
    app.MapControllerRoute(
        name: "DefaultHome",
        pattern: "{action}/{id?}",
        defaults: new { controller = "Home", action = "Index" },
        constraints: new { action = @"^(?!Admin)[a-zA-Z]+$" });

    // Default
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application startup failed");
    throw;
}
finally
{
    LogManager.Shutdown();
}
