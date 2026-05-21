using System.Threading;
using System.Threading.Tasks;
using YuvaTravel.Data.Uow.Repositories;
using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow
{
    /// <summary>
    /// 新版 EFUnitOfWork。Repository 由 UoW 自己 lazy new (B-1 模式),
    /// 不再走 services.Scan 自動掃描 — Phase B 全部 controller 遷移完成後,
    /// 舊的 services.Scan 註冊與舊 Repository 檔案才會被砍掉。
    /// </summary>
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly YuvaTravelDbContext _db;

        public EFUnitOfWork(YuvaTravelDbContext db)
        {
            _db = db;
        }

        public void Commit() => _db.SaveChanges();

        public Task CommitAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

        private IAuthorRepository _authorRepo;
        public IAuthorRepository AuthorRepo => _authorRepo ??= new AuthorRepository(_db);

        private IGuideRepository _guideRepo;
        public IGuideRepository GuideRepo => _guideRepo ??= new GuideRepository(_db);

        private ISubscriberRepository _subscriberRepo;
        public ISubscriberRepository SubscriberRepo => _subscriberRepo ??= new SubscriberRepository(_db);

        private ITagRepository _tagRepo;
        public ITagRepository TagRepo => _tagRepo ??= new TagRepository(_db);

        private IRegionRepository _regionRepo;
        public IRegionRepository RegionRepo => _regionRepo ??= new RegionRepository(_db);

        private IWebMetaRepository _webMetaRepo;
        public IWebMetaRepository WebMetaRepo => _webMetaRepo ??= new WebMetaRepository(_db);

        private IWebBannerRepository _webBannerRepo;
        public IWebBannerRepository WebBannerRepo => _webBannerRepo ??= new WebBannerRepository(_db);

        private IArticleCategoryRepository _articleCategoryRepo;
        public IArticleCategoryRepository ArticleCategoryRepo => _articleCategoryRepo ??= new ArticleCategoryRepository(_db);

        private IQuestionAnswerRepository _questionAnswerRepo;
        public IQuestionAnswerRepository QuestionAnswerRepo => _questionAnswerRepo ??= new QuestionAnswerRepository(_db);

        private IUserRepository _userRepo;
        public IUserRepository UserRepo => _userRepo ??= new UserRepository(_db);

        private IUserGroupFuncProgramRepository _userGroupFuncProgramRepo;
        public IUserGroupFuncProgramRepository UserGroupFuncProgramRepo => _userGroupFuncProgramRepo ??= new UserGroupFuncProgramRepository(_db);

        private IUrlReferrerRepository _urlReferrerRepo;
        public IUrlReferrerRepository UrlReferrerRepo => _urlReferrerRepo ??= new UrlReferrerRepository(_db);

        private IUrlReferrerCodeRepository _urlReferrerCodeRepo;
        public IUrlReferrerCodeRepository UrlReferrerCodeRepo => _urlReferrerCodeRepo ??= new UrlReferrerCodeRepository(_db);

        private IWebConfigRepository _webConfigRepo;
        public IWebConfigRepository WebConfigRepo => _webConfigRepo ??= new WebConfigRepository(_db);

        private IUserLoginProviderRepository _userLoginProviderRepo;
        public IUserLoginProviderRepository UserLoginProviderRepo => _userLoginProviderRepo ??= new UserLoginProviderRepository(_db);

        private IUserForgetPasswordRepository _userForgetPasswordRepo;
        public IUserForgetPasswordRepository UserForgetPasswordRepo => _userForgetPasswordRepo ??= new UserForgetPasswordRepository(_db);

        private IFuncGroupRepository _funcGroupRepo;
        public IFuncGroupRepository FuncGroupRepo => _funcGroupRepo ??= new FuncGroupRepository(_db);

        private IFuncProgramRepository _funcProgramRepo;
        public IFuncProgramRepository FuncProgramRepo => _funcProgramRepo ??= new FuncProgramRepository(_db);

        private IHotKeywordRepository _hotKeywordRepo;
        public IHotKeywordRepository HotKeywordRepo => _hotKeywordRepo ??= new HotKeywordRepository(_db);

        private IIpBlockingRepository _ipBlockingRepo;
        public IIpBlockingRepository IpBlockingRepo => _ipBlockingRepo ??= new IpBlockingRepository(_db);

        private IIpOpeningRepository _ipOpeningRepo;
        public IIpOpeningRepository IpOpeningRepo => _ipOpeningRepo ??= new IpOpeningRepository(_db);

        private IRecruitAgentRepository _recruitAgentRepo;
        public IRecruitAgentRepository RecruitAgentRepo => _recruitAgentRepo ??= new RecruitAgentRepository(_db);

        private IGuideFaqRepository _guideFaqRepo;
        public IGuideFaqRepository GuideFaqRepo => _guideFaqRepo ??= new GuideFaqRepository(_db);

        private IGuideReviewRepository _guideReviewRepo;
        public IGuideReviewRepository GuideReviewRepo => _guideReviewRepo ??= new GuideReviewRepository(_db);

        private IGuideRouteRepository _guideRouteRepo;
        public IGuideRouteRepository GuideRouteRepo => _guideRouteRepo ??= new GuideRouteRepository(_db);

        private IGuideTagRepository _guideTagRepo;
        public IGuideTagRepository GuideTagRepo => _guideTagRepo ??= new GuideTagRepository(_db);

        private IGuideRegionRepository _guideRegionRepo;
        public IGuideRegionRepository GuideRegionRepo => _guideRegionRepo ??= new GuideRegionRepository(_db);

        private IUserGroupRepository _userGroupRepo;
        public IUserGroupRepository UserGroupRepo => _userGroupRepo ??= new UserGroupRepository(_db);

        private IArticleCommentRepository _articleCommentRepo;
        public IArticleCommentRepository ArticleCommentRepo => _articleCommentRepo ??= new ArticleCommentRepository(_db);

        private IArticleRepository _articleRepo;
        public IArticleRepository ArticleRepo => _articleRepo ??= new ArticleRepository(_db);

        private IArticleTagRepository _articleTagRepo;
        public IArticleTagRepository ArticleTagRepo => _articleTagRepo ??= new ArticleTagRepository(_db);

        private IArticleCategoryArticleRepository _articleCategoryArticleRepo;
        public IArticleCategoryArticleRepository ArticleCategoryArticleRepo => _articleCategoryArticleRepo ??= new ArticleCategoryArticleRepository(_db);

        private IArticleRegionRepository _articleRegionRepo;
        public IArticleRegionRepository ArticleRegionRepo => _articleRegionRepo ??= new ArticleRegionRepository(_db);

        private IArticleSDCommonQuestionRepository _articleSDCommonQuestionRepo;
        public IArticleSDCommonQuestionRepository ArticleSDCommonQuestionRepo => _articleSDCommonQuestionRepo ??= new ArticleSDCommonQuestionRepository(_db);

        private IArticleViewRepository _articleViewRepo;
        public IArticleViewRepository ArticleViewRepo => _articleViewRepo ??= new ArticleViewRepository(_db);

        private IRecordClickRepository _recordClickRepo;
        public IRecordClickRepository RecordClickRepo => _recordClickRepo ??= new RecordClickRepository(_db);
    }
}
