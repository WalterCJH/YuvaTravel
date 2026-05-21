using System.Threading;
using System.Threading.Tasks;
using YuvaTravel.Data.Uow.IRepositories;

namespace YuvaTravel.Data.Uow
{
    /// <summary>
    /// UnitOfWork。每個 Repository 以 lazy property 公開,共用同一個 DbContext。
    /// </summary>
    public interface IUnitOfWork
    {
        void Commit();
        Task CommitAsync(CancellationToken ct = default);

        IAuthorRepository AuthorRepo { get; }
        IGuideRepository GuideRepo { get; }
        ISubscriberRepository SubscriberRepo { get; }
        ITagRepository TagRepo { get; }
        IRegionRepository RegionRepo { get; }
        IWebMetaRepository WebMetaRepo { get; }
        IWebBannerRepository WebBannerRepo { get; }
        IArticleCategoryRepository ArticleCategoryRepo { get; }
        IQuestionAnswerRepository QuestionAnswerRepo { get; }

        // BaseController 共用的 5 個 repo
        IUserRepository UserRepo { get; }
        IUserGroupFuncProgramRepository UserGroupFuncProgramRepo { get; }
        IUrlReferrerRepository UrlReferrerRepo { get; }
        IUrlReferrerCodeRepository UrlReferrerCodeRepo { get; }
        IWebConfigRepository WebConfigRepo { get; }

        // 使用者衍生
        IUserLoginProviderRepository UserLoginProviderRepo { get; }
        IUserForgetPasswordRepository UserForgetPasswordRepo { get; }

        // 第二輪簡單 CRUD
        IFuncGroupRepository FuncGroupRepo { get; }
        IFuncProgramRepository FuncProgramRepo { get; }
        IHotKeywordRepository HotKeywordRepo { get; }
        IIpBlockingRepository IpBlockingRepo { get; }
        IIpOpeningRepository IpOpeningRepo { get; }
        IRecruitAgentRepository RecruitAgentRepo { get; }

        // Guide 子實體
        IGuideFaqRepository GuideFaqRepo { get; }
        IGuideReviewRepository GuideReviewRepo { get; }
        IGuideRouteRepository GuideRouteRepo { get; }
        IGuideTagRepository GuideTagRepo { get; }
        IGuideRegionRepository GuideRegionRepo { get; }

        // 權限管理
        IUserGroupRepository UserGroupRepo { get; }

        // 文章 & 子實體
        IArticleRepository ArticleRepo { get; }
        IArticleCommentRepository ArticleCommentRepo { get; }
        IArticleTagRepository ArticleTagRepo { get; }
        IArticleCategoryArticleRepository ArticleCategoryArticleRepo { get; }
        IArticleRegionRepository ArticleRegionRepo { get; }
        IArticleSDCommonQuestionRepository ArticleSDCommonQuestionRepo { get; }
        IArticleViewRepository ArticleViewRepo { get; }

        // 點擊紀錄
        IRecordClickRepository RecordClickRepo { get; }
    }
}
