using YuvaTravel.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace YuvaTravel.Data
{
    public class YuvaTravelDbContext : DbContext
    {
        public YuvaTravelDbContext(DbContextOptions<YuvaTravelDbContext> options) : base(options)
        {
        }

        public virtual DbSet<IpBlocking> IpBlockings { get; set; }
        public virtual DbSet<IpOpening> IpOpenings { get; set; }

        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<ArticleCategory> ArticleCategories { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleTag> ArticleTags { get; set; }
        public virtual DbSet<ArticleView> ArticleViews { get; set; }
        public virtual DbSet<ArticleComment> ArticleComments { get; set; }
        public virtual DbSet<ArticleSDCommonQuestion> ArticleSDCommonQuestions { get; set; }

        public virtual DbSet<WebConfig> WebConfigs { get; set; }
        public virtual DbSet<WebMeta> WebMetas { get; set; }
        public virtual DbSet<WebBanner> WebBanners { get; set; }
        public virtual DbSet<QuestionAnswer> QuestionAnswers { get; set; }

        public virtual DbSet<UrlReferrer> UrlReferrers { get; set; }
        public virtual DbSet<RecordClick> RecordClicks { get; set; }

        public virtual DbSet<HotKeyword> HotKeywords { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserForgetPassword> UserForgetPasswords { get; set; }
        public virtual DbSet<UserLoginProvider> UserLoginProviders { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserGroupFuncProgram> UserGroupFuncPrograms { get; set; }

        public virtual DbSet<FuncGroup> FuncGroups { get; set; }
        public virtual DbSet<FuncProgram> FuncPrograms { get; set; }

        public virtual DbSet<RecruitAgent> RecruitAgents { get; set; }

        public virtual DbSet<Guide> Guides { get; set; }
        public virtual DbSet<GuideTag> GuideTags { get; set; }
        public virtual DbSet<GuideRoute> GuideRoutes { get; set; }
        public virtual DbSet<GuideReview> GuideReviews { get; set; }
        public virtual DbSet<GuideFaq> GuideFaqs { get; set; }

        public virtual DbSet<Author> Authors { get; set; }

        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<GuideRegion> GuideRegions { get; set; }
        public virtual DbSet<ArticleRegion> ArticleRegions { get; set; }

        public virtual DbSet<Subscriber> Subscribers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Email 普通 index — 同一信箱可有多筆 (記錄每次訂閱事件,unsub 後重新訂閱會新增另一筆)
            modelBuilder.Entity<Subscriber>()
                .HasIndex(p => p.Email);
        }
    }
}
