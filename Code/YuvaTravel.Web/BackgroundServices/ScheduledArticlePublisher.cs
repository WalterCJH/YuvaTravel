using YuvaTravel.Base.Enum;
using YuvaTravel.Data.Uow;

namespace YuvaTravel.Web.BackgroundServices
{
    /// <summary>
    /// 文章上線排程。每隔設定的間隔,把已到 OnScheduleTime 的「排程中」文章轉成「已通過」。
    /// 純 .NET 內建(BackgroundService + PeriodicTimer),無外部套件。
    /// OnlineTime 在審核時已算好 = Max(OnScheduleTime, ReviewTime),這裡只翻狀態,不需再動 OnlineTime。
    /// Log 用 DI 注入的 ILogger(經 UseNLog 寫入既有 nlog.config 的 NLogs/{date}/Info.log、Error.log)。
    /// </summary>
    public class ScheduledArticlePublisher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ScheduledArticlePublisher> _logger;
        private readonly TimeSpan _interval;

        public ScheduledArticlePublisher(
            IServiceScopeFactory scopeFactory,
            ILogger<ScheduledArticlePublisher> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var minutes = configuration.GetValue<int?>("Scheduling:ArticlePublishIntervalMinutes") ?? 1;
            if (minutes < 1) minutes = 1;
            _interval = TimeSpan.FromMinutes(minutes);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_interval);

            // 啟動先跑一次,把停機期間錯過上線時間的文章補上線;之後每個間隔再跑。
            do
            {
                await PublishDueArticlesAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task PublishDueArticlesAsync(CancellationToken ct)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // QueryOnScheduleArticleAsync: 排程中 && OnScheduleTime < now,且為 tracking
                var dueArticles = await uow.ArticleRepo.QueryOnScheduleArticleAsync();
                if (dueArticles.Count == 0) return;

                foreach (var article in dueArticles)
                {
                    article.ArticleReviewType = ArticleReviewType.已通過;
                    if (article.OnlineTime == null) article.OnlineTime = DateTime.Now;
                }

                await uow.CommitAsync(ct);

                _logger.LogInformation(
                    "文章上線排程:已上線 {Count} 篇 ({Ids})",
                    dueArticles.Count,
                    string.Join(", ", dueArticles.Select(a => a.ArticleId)));
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // 應用關閉中,正常結束,不視為錯誤
            }
            catch (Exception ex)
            {
                // 單次失敗不可讓背景迴圈死掉;下個 tick 會再撈
                // (查詢條件 OnScheduleTime < now 會自動把這批補上)
                _logger.LogError(ex, "文章上線排程執行失敗,將於下次輪詢重試");
            }
        }
    }
}
