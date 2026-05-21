using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YuvaTravel.Data.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleCategories",
                columns: table => new
                {
                    ArticleCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: true),
                    Code = table.Column<string>(type: "VARCHAR(10)", maxLength: 10, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NameEn = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCategories", x => x.ArticleCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "FuncGroups",
                columns: table => new
                {
                    FuncGroupId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IconClass = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: false),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuncGroups", x => x.FuncGroupId);
                });

            migrationBuilder.CreateTable(
                name: "Guides",
                columns: table => new
                {
                    GuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NameEn = table.Column<string>(type: "VARCHAR(80)", maxLength: 80, nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    Languages = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LongBio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuoteText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    QuoteBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PortraitUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CoverUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    ServiceFormat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MinPeople = table.Column<int>(type: "int", nullable: true),
                    MaxPeople = table.Column<int>(type: "int", nullable: true),
                    ResponseTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartingPrice = table.Column<int>(type: "int", nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guides", x => x.GuideId);
                });

            migrationBuilder.CreateTable(
                name: "HotKeywords",
                columns: table => new
                {
                    HotKeywordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotKeywords", x => x.HotKeywordId);
                });

            migrationBuilder.CreateTable(
                name: "IpBlockings",
                columns: table => new
                {
                    IpBlockingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsIncompleteIP = table.Column<bool>(type: "bit", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpBlockings", x => x.IpBlockingId);
                });

            migrationBuilder.CreateTable(
                name: "IpOpenings",
                columns: table => new
                {
                    IpOpeningId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsIncompleteIP = table.Column<bool>(type: "bit", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpOpenings", x => x.IpOpeningId);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswers",
                columns: table => new
                {
                    QuestionAnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "VARCHAR(4000)", maxLength: 4000, nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswers", x => x.QuestionAnswerId);
                });

            migrationBuilder.CreateTable(
                name: "RecordClicks",
                columns: table => new
                {
                    RecordClickId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpAddress = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true),
                    ClickUrl = table.Column<string>(type: "VARCHAR(1000)", maxLength: 1000, nullable: true),
                    ClickTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UrlReferrer = table.Column<string>(type: "VARCHAR(1000)", maxLength: 1000, nullable: true),
                    ClickMemo = table.Column<string>(type: "NVARCHAR(200)", maxLength: 200, nullable: true),
                    IsMobile = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordClicks", x => x.RecordClickId);
                });

            migrationBuilder.CreateTable(
                name: "RecruitAgents",
                columns: table => new
                {
                    RecruitAgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nick = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TelegramID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LineID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Other = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsContact = table.Column<bool>(type: "bit", nullable: false),
                    ContactTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitAgents", x => x.RecruitAgentId);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Code = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NameEn = table.Column<string>(type: "VARCHAR(80)", maxLength: 80, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.RegionId);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    SubscriberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(254)", maxLength: 254, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UnsubscribeToken = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscribeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnsubscribedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SourcePath = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    SourceUrl = table.Column<string>(type: "VARCHAR(1000)", maxLength: 1000, nullable: true),
                    Referrer = table.Column<string>(type: "VARCHAR(1000)", maxLength: 1000, nullable: true),
                    IpAddress = table.Column<string>(type: "VARCHAR(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    AcceptLanguage = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    UtmSource = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    UtmMedium = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    UtmCampaign = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.SubscriberId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ColorCode = table.Column<string>(type: "CHAR(7)", maxLength: 7, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "UrlReferrerCode",
                columns: table => new
                {
                    UrlReferrerCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlReferrerCode", x => x.UrlReferrerCodeId);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    UserGroupId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AuthorizeLevel = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.UserGroupId);
                });

            migrationBuilder.CreateTable(
                name: "WebBanners",
                columns: table => new
                {
                    WebBannerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeginTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageUrl = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    ImageMobileUrl = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    ImageTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImageAlt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImageClickUrl = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebBanners", x => x.WebBannerId);
                });

            migrationBuilder.CreateTable(
                name: "WebConfigs",
                columns: table => new
                {
                    WebConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoRegister = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebConfigs", x => x.WebConfigId);
                });

            migrationBuilder.CreateTable(
                name: "WebMetas",
                columns: table => new
                {
                    WebMetaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    OgType = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: false),
                    MetaTitle = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    MetaDescription = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: true),
                    MetaImageUrl = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    Canonical = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: true),
                    IsCanonicalAbsoluteUri = table.Column<bool>(type: "bit", nullable: false),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebMetas", x => x.WebMetaId);
                });

            migrationBuilder.CreateTable(
                name: "FuncPrograms",
                columns: table => new
                {
                    FuncProgramId = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    FuncGroupId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: false),
                    IsActiveCreate = table.Column<bool>(type: "bit", nullable: false),
                    IsActiveEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsActiveDelete = table.Column<bool>(type: "bit", nullable: false),
                    IsActiveDetails = table.Column<bool>(type: "bit", nullable: false),
                    IsActiveImport = table.Column<bool>(type: "bit", nullable: false),
                    IsActiveExport = table.Column<bool>(type: "bit", nullable: false),
                    IsChangeCreate = table.Column<bool>(type: "bit", nullable: false),
                    IsChangeEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsChangeDelete = table.Column<bool>(type: "bit", nullable: false),
                    IsChangeDetails = table.Column<bool>(type: "bit", nullable: false),
                    IsChangeImport = table.Column<bool>(type: "bit", nullable: false),
                    IsChangeExport = table.Column<bool>(type: "bit", nullable: false),
                    AuthorizeLevel = table.Column<int>(type: "int", nullable: false),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    OnlyAdmin = table.Column<bool>(type: "bit", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuncPrograms", x => x.FuncProgramId);
                    table.ForeignKey(
                        name: "FK_FuncPrograms_FuncGroups_FuncGroupId",
                        column: x => x.FuncGroupId,
                        principalTable: "FuncGroups",
                        principalColumn: "FuncGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.AuthorId);
                    table.ForeignKey(
                        name: "FK_Authors_Guides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guides",
                        principalColumn: "GuideId");
                });

            migrationBuilder.CreateTable(
                name: "GuideFaqs",
                columns: table => new
                {
                    GuideFaqId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideFaqs", x => x.GuideFaqId);
                    table.ForeignKey(
                        name: "FK_GuideFaqs_Guides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guides",
                        principalColumn: "GuideId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuideReviews",
                columns: table => new
                {
                    GuideReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReviewerFrom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Stars = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TripDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TripDays = table.Column<int>(type: "int", nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideReviews", x => x.GuideReviewId);
                    table.ForeignKey(
                        name: "FK_GuideReviews_Guides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guides",
                        principalColumn: "GuideId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuideRoutes",
                columns: table => new
                {
                    GuideRouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Url = table.Column<string>(type: "VARCHAR(200)", maxLength: 200, nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideRoutes", x => x.GuideRouteId);
                    table.ForeignKey(
                        name: "FK_GuideRoutes_Guides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guides",
                        principalColumn: "GuideId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuideRegions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuideRegions_Guides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guides",
                        principalColumn: "GuideId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuideRegions_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuideTags",
                columns: table => new
                {
                    GuideTagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuideId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideTags", x => x.GuideTagId);
                    table.ForeignKey(
                        name: "FK_GuideTags_Guides_GuideId",
                        column: x => x.GuideId,
                        principalTable: "Guides",
                        principalColumn: "GuideId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuideTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrlReferrers",
                columns: table => new
                {
                    UrlReferrerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UrlReferrerCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferrerCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DestinationUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CloudFlareIpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsMobile = table.Column<bool>(type: "bit", nullable: false),
                    OS = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Platform = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BrowserType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BrowserVersion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsOnActionExecuting = table.Column<bool>(type: "bit", nullable: false),
                    IsOnActionExecuted = table.Column<bool>(type: "bit", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlReferrers", x => x.UrlReferrerId);
                    table.ForeignKey(
                        name: "FK_UrlReferrers_UrlReferrerCode_UrlReferrerCodeId",
                        column: x => x.UrlReferrerCodeId,
                        principalTable: "UrlReferrerCode",
                        principalColumn: "UrlReferrerCodeId");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: false),
                    UserGroupId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    LoginResetPassword = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Salutation = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Phone_AreaNo = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Phone_No = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Phone_Ext = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Email = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "VARCHAR(128)", maxLength: 128, nullable: true),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserGuid);
                    table.ForeignKey(
                        name: "FK_Users_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "UserGroupId");
                });

            migrationBuilder.CreateTable(
                name: "UserGroupFuncPrograms",
                columns: table => new
                {
                    UserGroupFuncProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserGroupId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    FuncProgramId = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: true),
                    IsCreate = table.Column<bool>(type: "bit", nullable: false),
                    IsEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    IsDetails = table.Column<bool>(type: "bit", nullable: false),
                    IsImport = table.Column<bool>(type: "bit", nullable: false),
                    IsExport = table.Column<bool>(type: "bit", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupFuncPrograms", x => x.UserGroupFuncProgramId);
                    table.ForeignKey(
                        name: "FK_UserGroupFuncPrograms_FuncPrograms_FuncProgramId",
                        column: x => x.FuncProgramId,
                        principalTable: "FuncPrograms",
                        principalColumn: "FuncProgramId");
                    table.ForeignKey(
                        name: "FK_UserGroupFuncPrograms_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "UserGroupId");
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleType = table.Column<int>(type: "int", nullable: false),
                    ArticleReviewType = table.Column<int>(type: "int", nullable: false),
                    IsTop = table.Column<bool>(type: "bit", nullable: false),
                    IsFeaturedArticle = table.Column<bool>(type: "bit", nullable: false),
                    FeaturedArticleDisplaySeq = table.Column<int>(type: "int", nullable: false),
                    PromotionCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SerialNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageHomePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageCategoryPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageMobilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageBannerMobilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImageAlt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    ReadTimeMin = table.Column<int>(type: "int", nullable: true),
                    ReviewUserId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ReviewTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OnScheduleTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OnlineTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventLocation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EventStarNum = table.Column<int>(type: "int", nullable: true),
                    EventCount = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EventPrice = table.Column<int>(type: "int", nullable: true),
                    EventDiscount = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EventDiscountValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.ArticleId);
                    table.ForeignKey(
                        name: "FK_Articles_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "AuthorId");
                });

            migrationBuilder.CreateTable(
                name: "UserForgetPasswords",
                columns: table => new
                {
                    ForgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserForgetPasswords", x => x.ForgetId);
                    table.ForeignKey(
                        name: "FK_UserForgetPasswords_Users_UserGuid",
                        column: x => x.UserGuid,
                        principalTable: "Users",
                        principalColumn: "UserGuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginProviders",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    UserGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginProviders", x => new { x.LoginProvider, x.ProviderKey, x.UserGuid });
                    table.ForeignKey(
                        name: "FK_UserLoginProviders_Users_UserGuid",
                        column: x => x.UserGuid,
                        principalTable: "Users",
                        principalColumn: "UserGuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleCategoryArticle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCategoryArticle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleCategoryArticle_ArticleCategories_ArticleCategoryId",
                        column: x => x.ArticleCategoryId,
                        principalTable: "ArticleCategories",
                        principalColumn: "ArticleCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleCategoryArticle_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleComments",
                columns: table => new
                {
                    ArticleCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentContent = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: true),
                    CommentUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CommentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReplyContent = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: true),
                    ReplyUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    ReplyTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleComments", x => x.ArticleCommentId);
                    table.ForeignKey(
                        name: "FK_ArticleComments_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleRegions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUserId = table.Column<string>(type: "VARCHAR(30)", maxLength: 30, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleRegions_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleRegions_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleSDCommonQuestions",
                columns: table => new
                {
                    ArticleSDCommonQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplaySequence = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleSDCommonQuestions", x => x.ArticleSDCommonQuestionId);
                    table.ForeignKey(
                        name: "FK_ArticleSDCommonQuestions_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleTags",
                columns: table => new
                {
                    ArticleTagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplaySeq = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTags", x => x.ArticleTagId);
                    table.ForeignKey(
                        name: "FK_ArticleTags_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleViews",
                columns: table => new
                {
                    ArticleViewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleViews", x => x.ArticleViewId);
                    table.ForeignKey(
                        name: "FK_ArticleViews_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategoryArticle_ArticleCategoryId",
                table: "ArticleCategoryArticle",
                column: "ArticleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategoryArticle_ArticleId",
                table: "ArticleCategoryArticle",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleComments_ArticleId",
                table: "ArticleComments",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleRegions_ArticleId",
                table: "ArticleRegions",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleRegions_RegionId",
                table: "ArticleRegions",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleSDCommonQuestions_ArticleId",
                table: "ArticleSDCommonQuestions",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTags_ArticleId",
                table: "ArticleTags",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTags_TagId",
                table: "ArticleTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleViews_ArticleId",
                table: "ArticleViews",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_GuideId",
                table: "Authors",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_FuncPrograms_FuncGroupId",
                table: "FuncPrograms",
                column: "FuncGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideFaqs_GuideId",
                table: "GuideFaqs",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideRegions_GuideId",
                table: "GuideRegions",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideRegions_RegionId",
                table: "GuideRegions",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideReviews_GuideId",
                table: "GuideReviews",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideRoutes_GuideId",
                table: "GuideRoutes",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideTags_GuideId",
                table: "GuideTags",
                column: "GuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideTags_TagId",
                table: "GuideTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_Email",
                table: "Subscribers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_UrlReferrers_UrlReferrerCodeId",
                table: "UrlReferrers",
                column: "UrlReferrerCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserForgetPasswords_UserGuid",
                table: "UserForgetPasswords",
                column: "UserGuid");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupFuncPrograms_FuncProgramId",
                table: "UserGroupFuncPrograms",
                column: "FuncProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupFuncPrograms_UserGroupId",
                table: "UserGroupFuncPrograms",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginProviders_UserGuid",
                table: "UserLoginProviders",
                column: "UserGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserGroupId",
                table: "Users",
                column: "UserGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleCategoryArticle");

            migrationBuilder.DropTable(
                name: "ArticleComments");

            migrationBuilder.DropTable(
                name: "ArticleRegions");

            migrationBuilder.DropTable(
                name: "ArticleSDCommonQuestions");

            migrationBuilder.DropTable(
                name: "ArticleTags");

            migrationBuilder.DropTable(
                name: "ArticleViews");

            migrationBuilder.DropTable(
                name: "GuideFaqs");

            migrationBuilder.DropTable(
                name: "GuideRegions");

            migrationBuilder.DropTable(
                name: "GuideReviews");

            migrationBuilder.DropTable(
                name: "GuideRoutes");

            migrationBuilder.DropTable(
                name: "GuideTags");

            migrationBuilder.DropTable(
                name: "HotKeywords");

            migrationBuilder.DropTable(
                name: "IpBlockings");

            migrationBuilder.DropTable(
                name: "IpOpenings");

            migrationBuilder.DropTable(
                name: "QuestionAnswers");

            migrationBuilder.DropTable(
                name: "RecordClicks");

            migrationBuilder.DropTable(
                name: "RecruitAgents");

            migrationBuilder.DropTable(
                name: "Subscribers");

            migrationBuilder.DropTable(
                name: "UrlReferrers");

            migrationBuilder.DropTable(
                name: "UserForgetPasswords");

            migrationBuilder.DropTable(
                name: "UserGroupFuncPrograms");

            migrationBuilder.DropTable(
                name: "UserLoginProviders");

            migrationBuilder.DropTable(
                name: "WebBanners");

            migrationBuilder.DropTable(
                name: "WebConfigs");

            migrationBuilder.DropTable(
                name: "WebMetas");

            migrationBuilder.DropTable(
                name: "ArticleCategories");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "UrlReferrerCode");

            migrationBuilder.DropTable(
                name: "FuncPrograms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "FuncGroups");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "Guides");
        }
    }
}
