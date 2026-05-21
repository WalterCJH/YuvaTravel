-- ============================================================
-- 假資料 seed:ArticleCategories / Articles / Guides + 子表
-- 全部用 NEWID() 產生 GUID,並以 DECLARE 變數串接關聯
--
-- 執行前提:
--   1. 資料庫已套用 AddGuideEntities migration
--   2. 重跑前請先 DELETE 舊資料 (NEWID 每次都產新 GUID)
-- ============================================================
SET NOCOUNT ON;
DECLARE @Now DATETIME = GETDATE();
DECLARE @UserId NVARCHAR(30) = N'System';

-- ============================================================
-- 1) ArticleCategorie (4 筆)
-- ============================================================
DECLARE @CatKyoto    UNIQUEIDENTIFIER = NEWID();
DECLARE @CatTainan   UNIQUEIDENTIFIER = NEWID();
DECLARE @CatChiangMai UNIQUEIDENTIFIER = NEWID();
DECLARE @CatTokyo    UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.ArticleCategorie
    (ArticleCategoryId, PageType, Url, Code, Name, NameEn,
     GoogleIconClass, IconClass, ShowMain, ShowBlockCategory, BlockCategoryDisplaySeq,
     ShowMobilFooter, MobileFooterDisplaySeq, ImageUrl, ImageMobileUrl, DisplaySeq,
     CreateUserId, CreateTime)
VALUES
    (@CatKyoto,    10, N'/category?c=kyoto',    'kyoto',    N'京都', 'Kyoto',
     'temple', 'fas fa-torii-gate', 1, 1, 10,
     1, 10, N'https://images.unsplash.com/photo-1493780474015-ba834fd0ce2f?auto=format&fit=crop&w=600&q=80', N'', 100,
     @UserId, @Now),

    (@CatTainan,   10, N'/category?c=tainan',   'tainan',   N'台南', 'Tainan',
     'restaurant', 'fas fa-utensils', 1, 1, 20,
     1, 20, N'https://images.unsplash.com/photo-1528164344705-47542687000d?auto=format&fit=crop&w=600&q=80', N'', 200,
     @UserId, @Now),

    (@CatChiangMai, 10, N'/category?c=chiangmai', 'chiangmai', N'清邁', 'Chiang Mai',
     'forest', 'fas fa-mountain', 1, 1, 30,
     0, 0, N'https://images.unsplash.com/photo-1578469645742-46cae010e5d4?auto=format&fit=crop&w=600&q=80', N'', 300,
     @UserId, @Now),

    (@CatTokyo,    10, N'/category?c=tokyo',    'tokyo',    N'東京', 'Tokyo',
     'storefront', 'fas fa-city', 1, 1, 40,
     0, 0, N'https://images.unsplash.com/photo-1528360983277-13d401cdc186?auto=format&fit=crop&w=600&q=80', N'', 400,
     @UserId, @Now);

-- ============================================================
-- 2) Articles (4 筆)  ArticleType=10(文章), ArticleReviewType=100(已通過)
-- ============================================================
DECLARE @ArtKyotoRain    UNIQUEIDENTIFIER = NEWID();
DECLARE @ArtTainanDawn   UNIQUEIDENTIFIER = NEWID();
DECLARE @ArtChiangMaiSun UNIQUEIDENTIFIER = NEWID();
DECLARE @ArtKyotoTorii   UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Articles
    (ArticleId, ArticleType, ArticleReviewType, IsTop, IsFeaturedArticle, FeaturedArticleDisplaySeq,
     PromotionCode, Author, Code, Title, Description, Content, Content2, Content3, Content4,
     MetaTitle, MetaDescription, ImagePath, ImageHomePath, ImageCategoryPath, ImageMobilePath,
     ImageBannerMobilePath, ImageTitle, ImageAlt, DisplaySeq, Views, ReadTimeMin,
     ReviewUserId, ReviewTime, OnScheduleTime, OnlineTime,
     EventLocation, EventStarNum, EventCount, EventPrice, EventDiscount, EventDiscountValue,
     CreateUserId, CreateTime)
VALUES
    (@ArtKyotoRain, 10, 100, 1, 1, 1,
     N'', N'美咲 Misaki', 'kyoto-rain-higashiyama',
     N'京都的雨．東山的茶',
     N'與美咲走一段,從一碗抹茶開始的雨天午後。',
     N'<p>下雨的時候才看得見東山的真實樣貌。</p><h2>東山的雨</h2><p>嚮導美咲帶我們繞過清水寺的人潮,走進一間沒有招牌的茶屋——窗外落雨,茶湯熱氣升起的那一刻,京都才真正進到心裡。</p>',
     N'', N'', N'',
     N'京都的雨．東山的茶 | 域見', N'下雨天的東山,京都真正的樣子才會出現。',
     N'https://images.unsplash.com/photo-1493780474015-ba834fd0ce2f?auto=format&fit=crop&w=2000&q=80',
     N'https://images.unsplash.com/photo-1493780474015-ba834fd0ce2f?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1493780474015-ba834fd0ce2f?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1493780474015-ba834fd0ce2f?auto=format&fit=crop&w=600&q=80',
     N'', N'雨後的東山', N'京都東山的雨後石板街道',
     100, 1820, 9, NULL, NULL, NULL, DATEADD(DAY, -25, @Now),
     N'', NULL, N'', NULL, N'', N'',
     @UserId, @Now),

    (@ArtTainanDawn, 10, 100, 0, 1, 2,
     N'', N'美玲 Mei-Ling', 'tainan-old-alley-dawn',
     N'台南老巷的清晨',
     N'與美玲走一段,從一碗牛肉湯開始的早晨。',
     N'<p>清晨五點半,老巷裡的霧還沒散。</p><h2>第一鍋湯</h2><p>湯是凌晨兩點開始熬的,三點半切肉,五點開店。六點以後,肉就不甜了。</p>',
     N'', N'', N'',
     N'台南老巷的清晨 | 域見', N'與美玲走一段,從一碗牛肉湯開始的早晨。',
     N'https://images.unsplash.com/photo-1551845041-63e8e76836ea?auto=format&fit=crop&w=2000&q=80',
     N'https://images.unsplash.com/photo-1551845041-63e8e76836ea?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1551845041-63e8e76836ea?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1551845041-63e8e76836ea?auto=format&fit=crop&w=600&q=80',
     N'', N'台南民族路', N'台南老巷裡的清晨',
     200, 2041, 8, NULL, NULL, NULL, DATEADD(DAY, -18, @Now),
     N'', NULL, N'', NULL, N'', N'',
     @UserId, @Now),

    (@ArtChiangMaiSun, 10, 100, 0, 0, 0,
     N'', N'阿諾 Arnon', 'chiangmai-sunday-market',
     N'清邁的星期天市場',
     N'阿諾帶你走一段,只有星期天才會出現的夜市。',
     N'<p>清邁的星期天,從一條巷子開始熱鬧。</p><h2>市場的氣味</h2><p>烤肉、香茅、椰糖的氣味交錯成這座城市最真實的呼吸。</p>',
     N'', N'', N'',
     N'清邁的星期天市場 | 域見', N'只有星期天才會出現的夜市,泰北的真實節奏。',
     N'https://images.unsplash.com/photo-1528181304800-259b08848526?auto=format&fit=crop&w=2000&q=80',
     N'https://images.unsplash.com/photo-1528181304800-259b08848526?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1528181304800-259b08848526?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1528181304800-259b08848526?auto=format&fit=crop&w=600&q=80',
     N'', N'清邁夜市', N'清邁星期天市場的烤肉攤',
     300, 2938, 6, NULL, NULL, NULL, DATEADD(DAY, -10, @Now),
     N'', NULL, N'', NULL, N'', N'',
     @UserId, @Now),

    (@ArtKyotoTorii, 10, 100, 0, 0, 0,
     N'', N'美咲 Misaki', 'kyoto-fushimi-inari-dawn',
     N'在伏見稻荷遇見清晨第一道光',
     N'清晨五點半,千本鳥居還是冷冷的硃紅。',
     N'<p>嚮導美咲帶我們走觀光客不會走的後山小徑——當第一縷陽光從千本鳥居縫隙灑下來,連風都安靜了下來。</p>',
     N'', N'', N'',
     N'伏見稻荷的清晨 | 域見', N'清晨五點半,千本鳥居還是冷冷的硃紅。',
     N'https://images.unsplash.com/photo-1528360983277-13d401cdc186?auto=format&fit=crop&w=2000&q=80',
     N'https://images.unsplash.com/photo-1528360983277-13d401cdc186?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1528360983277-13d401cdc186?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1528360983277-13d401cdc186?auto=format&fit=crop&w=600&q=80',
     N'', N'伏見稻荷', N'清晨的伏見稻荷千本鳥居',
     400, 2408, 7, NULL, NULL, NULL, DATEADD(DAY, -5, @Now),
     N'', NULL, N'', NULL, N'', N'',
     @UserId, @Now);

-- ============================================================
-- 3) ArticleCategoryArticles (文章 - 分類關聯)
-- ============================================================
INSERT INTO dbo.ArticleCategoryArticles (Id, ArticleCategoryId, ArticleId, CreateUserId, CreateTime)
VALUES
    (NEWID(), @CatKyoto,     @ArtKyotoRain,    @UserId, @Now),
    (NEWID(), @CatTainan,    @ArtTainanDawn,   @UserId, @Now),
    (NEWID(), @CatChiangMai, @ArtChiangMaiSun, @UserId, @Now),
    (NEWID(), @CatKyoto,     @ArtKyotoTorii,   @UserId, @Now);

-- ============================================================
-- 4) Guides (4 筆)
-- ============================================================
DECLARE @GuideMisaki  UNIQUEIDENTIFIER = NEWID();
DECLARE @GuideMeiLing UNIQUEIDENTIFIER = NEWID();
DECLARE @GuideArnon   UNIQUEIDENTIFIER = NEWID();
DECLARE @GuideKenichi UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Guides
    (GuideId, IsActive, IsFeatured, Code, Name, NameEn, City, CityCode,
     YearsOfExperience, Languages, Bio, LongBio, QuoteText, QuoteBy,
     PortraitUrl, CoverUrl, Rating, ReviewCount,
     ServiceFormat, MinPeople, MaxPeople, ResponseTime, StartingPrice, DisplaySeq,
     CreateUserId, CreateTime)
VALUES
    (@GuideMisaki, 1, 1, 'misaki',
     N'美咲', N'Misaki Tanaka', N'京都', 'kyoto',
     7, N'JP · EN',
     N'在東山長大,最熟下雨天的茶屋路線。',
     N'<p>我母親在祇園開了 30 年的茶屋。從小我就在榻榻米上爬,看著大人們點茶、論詩。</p><p>我做嚮導的方式不像別人。我不會讓你走完一張清單,而是會在你坐下來喝第一口茶的時候,讓你忽然懂得:京都不是用看的,是用慢的。</p>',
     N'我喜歡在下雨天帶旅人走東山,京都真正的樣子,在那時候才會出現。',
     N'美咲,2026 春',
     N'https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1545569341-9eb8b30979d9?auto=format&fit=crop&w=1200&h=508&q=80',
     4.9, 38, N'半日 / 全日', 1, 4, N'24 小時內', 4800, 100,
     @UserId, @Now),

    (@GuideMeiLing, 1, 1, 'meiling',
     N'美玲', N'Mei-Ling Chen', N'台南', 'tainan',
     5, N'ZH · EN',
     N'「我帶你走的,是我阿嬤從小走的路。」最懂台南老巷的清晨味道與牛肉湯的早餐文化。',
     N'<p>在老巷長大,走了 20 年。早晨先帶你吃一碗牛肉湯,再用一整個上午,慢慢走進這個城市的縫隙。</p>',
     N'巷子的盡頭不是出口 — 是另一條巷子的開頭。',
     N'美玲,2026 夏',
     N'https://images.unsplash.com/photo-1531123897727-8f129e1688ce?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1551845041-63e8e76836ea?auto=format&fit=crop&w=1200&h=508&q=80',
     5.0, 42, N'半日 / 全日', 1, 6, N'12 小時內', 3800, 200,
     @UserId, @Now),

    (@GuideArnon, 1, 0, 'arnon',
     N'阿諾', N'Arnon Pongpat', N'清邁', 'chiangmai',
     9, N'TH · EN',
     N'山徑、市場、夜晚都市的小角落 — 帶旅人走進泰北的真實節奏,不只是觀光路線。',
     N'<p>我在清邁長大,做嚮導 9 年了。我帶的路線從不重複,因為清邁每天都在變。</p><p>星期天的市場、雨季的山徑、深夜的小料理屋——這些不在觀光地圖上的角落,才是清邁真正的樣子。</p>',
     N'觀光地圖上沒有的地方,才是真實的清邁。',
     N'阿諾,2026 春',
     N'https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1578469645742-46cae010e5d4?auto=format&fit=crop&w=1200&h=508&q=80',
     4.9, 27, N'半日 / 全日 / 多日', 1, 8, N'24 小時內', 3200, 300,
     @UserId, @Now),

    (@GuideKenichi, 1, 0, 'kenichi',
     N'健一', N'Ken''ichi Sato', N'京都', 'kyoto',
     12, N'JP · EN',
     N'夜晚的神社、人煙稀少的山徑。',
     N'<p>京都人。本業是攝影師,做嚮導 12 年。喜歡夜晚的神社、人煙稀少的山徑,還有沒有招牌的小料理屋。</p>',
     N'白日的祇園屬於觀光客,夜晚的祇園才屬於京都人。',
     N'健一,2025 秋',
     N'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=900&q=80',
     N'https://images.unsplash.com/photo-1480796927426-f609979314bd?auto=format&fit=crop&w=1200&h=508&q=80',
     4.8, 19, N'半日 / 全日', 1, 4, N'24 小時內', 5200, 400,
     @UserId, @Now);

-- ============================================================
-- 5) GuideRoutes (各嚮導 2-3 條路線,共 9 筆)
-- ============================================================
INSERT INTO dbo.GuideRoutes (GuideRouteId, GuideId, Title, Description, Duration, Url, DisplaySeq, CreateUserId, CreateTime)
VALUES
    -- 美咲
    (NEWID(), @GuideMisaki,
     N'雨天的東山',
     N'繞過清水寺人潮的茶屋路線。從寧寧之道走到二年坂,在雨聲裡停下來喝一碗抹茶。',
     N'半日 · 4 小時', N'', 10, @UserId, @Now),
    (NEWID(), @GuideMisaki,
     N'茶道入門',
     N'一個人也能參加的小席。在母親的茶屋裡,學第一口茶的禮儀。',
     N'半日 · 3 小時', N'', 20, @UserId, @Now),
    (NEWID(), @GuideMisaki,
     N'錦市場清晨',
     N'六點半,跟料亭的廚師一起逛攤。看京都人如何挑早晨的第一籃菜。',
     N'晨間 · 3 小時', N'', 30, @UserId, @Now),

    -- 美玲
    (NEWID(), @GuideMeiLing,
     N'老巷的清晨',
     N'從一碗牛肉湯開始的台南早晨。沿著民族路走七個轉角,看城市最早醒著的樣子。',
     N'晨間 · 3 小時', N'', 10, @UserId, @Now),
    (NEWID(), @GuideMeiLing,
     N'神農街與府城',
     N'從赤崁樓走到神農街,認識台南三百年的縫隙。',
     N'半日 · 4 小時', N'', 20, @UserId, @Now),

    -- 阿諾
    (NEWID(), @GuideArnon,
     N'星期天市場',
     N'清邁夜市的真實樣子。從第一攤烤肉走到最後一家手工藝攤,聽攤主的故事。',
     N'晚間 · 3 小時', N'', 10, @UserId, @Now),
    (NEWID(), @GuideArnon,
     N'素帖山日落',
     N'下午三點出發,在山頂等一場日落。回程順道走一段山徑。',
     N'半日 · 5 小時', N'', 20, @UserId, @Now),

    -- 健一
    (NEWID(), @GuideKenichi,
     N'祇園後巷夜走',
     N'白天的祇園屬於觀光客,夜晚才看得見真實的料亭與紅燈籠。',
     N'晚間 · 3 小時', N'', 10, @UserId, @Now),
    (NEWID(), @GuideKenichi,
     N'伏見稻荷後山',
     N'走觀光客不會走的後山小徑,看清晨第一道光穿過千本鳥居。',
     N'晨間 · 3 小時', N'', 20, @UserId, @Now);

-- ============================================================
-- 6) GuideReviews (各嚮導 2-3 則評論,共 9 筆)
-- ============================================================
INSERT INTO dbo.GuideReviews (GuideReviewId, GuideId, ReviewerName, ReviewerFrom, Stars, Content, TripDate, TripDays, DisplaySeq, CreateUserId, CreateTime)
VALUES
    -- 美咲
    (NEWID(), @GuideMisaki, N'Carol', N'台北', 5,
     N'那天剛好下雨,美咲沒有改行程,反而說:「太好了,京都最好的樣子要出現了。」整個下午我們撐傘走東山,幾乎沒遇到別的觀光客。',
     '2026-03-15', 3, 10, @UserId, @Now),
    (NEWID(), @GuideMisaki, N'Daniel', N'香港', 5,
     N'茶道的那一席,是我整趟旅程最安靜的兩個小時。回香港後一個禮拜,還收到她寄來的手寫信。',
     '2026-02-20', 2, 20, @UserId, @Now),
    (NEWID(), @GuideMisaki, N'Sarah', N'新加坡', 5,
     N'錦市場那場 6 點半的逛攤太棒了。看料亭廚師怎麼挑魚,是我這輩子最近距離看一個城市的清晨。',
     '2026-01-10', 4, 30, @UserId, @Now),

    -- 美玲
    (NEWID(), @GuideMeiLing, N'Wendy', N'台北', 5,
     N'阿嬤的牛肉湯、阿嬤的椪餅,還有阿嬤推薦的玉蘭花路口。整個早晨像走進美玲家的回憶。',
     '2026-04-05', 2, 10, @UserId, @Now),
    (NEWID(), @GuideMeiLing, N'Kevin', N'上海', 5,
     N'第一次來台南,沒想到能走進這麼多沒有招牌的小店。美玲熟到老闆都認識她。',
     '2026-03-22', 3, 20, @UserId, @Now),

    -- 阿諾
    (NEWID(), @GuideArnon, N'Mark', N'台北', 5,
     N'素帖山的日落超震撼,而且因為阿諾選的時間,完全沒有遊客團。',
     '2026-02-28', 5, 10, @UserId, @Now),
    (NEWID(), @GuideArnon, N'Yuki', N'東京', 4,
     N'夜市那場很好玩,阿諾的英文清楚易懂,跟攤主之間還會用泰文閒聊讓我們翻譯成中文。',
     '2026-01-15', 4, 20, @UserId, @Now),

    -- 健一
    (NEWID(), @GuideKenichi, N'Emma', N'美國', 5,
     N'夜晚的祇園完全是另一個城市。健一帶我們進去的小料理屋,菜單上沒有英文,但他翻譯得很細。',
     '2026-03-01', 4, 10, @UserId, @Now),
    (NEWID(), @GuideKenichi, N'阿志', N'台中', 5,
     N'伏見稻荷的清晨太美了,完全沒有人。健一也順便教我怎麼拍鳥居才不會空洞。',
     '2026-02-08', 3, 20, @UserId, @Now);

-- ============================================================
-- 7) GuideFaqs (各嚮導 2-3 題,共 9 筆)
-- ============================================================
INSERT INTO dbo.GuideFaqs (GuideFaqId, GuideId, Question, Answer, DisplaySeq, CreateUserId, CreateTime)
VALUES
    -- 美咲
    (NEWID(), @GuideMisaki,
     N'媒合美咲的流程要花多久?',
     N'<p>送出媒合表單後,美咲會在 24 小時內回覆你 — 通常會附上 2 條她建議的路線方向。確認後,我們會安排一次 30 分鐘的線上對談,讓你們先彼此認識。</p>',
     10, @UserId, @Now),
    (NEWID(), @GuideMisaki,
     N'如果天氣不好可以改期嗎?',
     N'<p>美咲的路線多數可以在雨天進行 (事實上她最愛雨天)。若是颱風或極端天氣,可在 48 小時前免費改期。</p>',
     20, @UserId, @Now),
    (NEWID(), @GuideMisaki,
     N'家庭旅人 / 帶小孩可以嗎?',
     N'<p>當然可以。請在媒合表單告訴我們小孩的年齡,美咲會調整路線的長度與步調。茶道入門特別適合家庭。</p>',
     30, @UserId, @Now),

    -- 美玲
    (NEWID(), @GuideMeiLing,
     N'早晨幾點開始?',
     N'<p>看你想看哪個樣子的台南。要看牛肉湯第一鍋,清晨 5 點;要看市場熱鬧,7 點;要慢慢走巷子,8 點半。</p>',
     10, @UserId, @Now),
    (NEWID(), @GuideMeiLing,
     N'素食 / 過敏可以嗎?',
     N'<p>當然可以。提前告訴我們你的飲食需求,美玲會調整推薦的攤位。</p>',
     20, @UserId, @Now),

    -- 阿諾
    (NEWID(), @GuideArnon,
     N'雨季可以去山徑嗎?',
     N'<p>雨季 (6–10 月) 的山徑會比較滑,但風景反而最美。阿諾會根據當天降雨量決定路線,安全第一。</p>',
     10, @UserId, @Now),
    (NEWID(), @GuideArnon,
     N'語言只能英文嗎?',
     N'<p>阿諾的主要語言是泰文和英文,但他很習慣面對亞洲旅人,溝通沒問題。需要中文翻譯也可以提前告訴我們安排。</p>',
     20, @UserId, @Now),

    -- 健一
    (NEWID(), @GuideKenichi,
     N'夜晚行程安全嗎?',
     N'<p>祇園夜晚的小巷其實很安靜,治安非常好。健一是當地人,熟到老闆都認識他,絕對安心。</p>',
     10, @UserId, @Now),
    (NEWID(), @GuideKenichi,
     N'可以幫忙拍照嗎?',
     N'<p>健一本業是攝影師,行程中如果你願意,他會用自己的相機幫你拍幾張,後製完寄給你。</p>',
     20, @UserId, @Now);

PRINT N'假資料 seed 完成:';
PRINT N'  - ArticleCategories: 4 筆 (京都 / 台南 / 清邁 / 東京)';
PRINT N'  - Articles: 4 筆 (京都的雨 / 台南清晨 / 清邁市場 / 伏見稻荷)';
PRINT N'  - ArticleCategoryArticles: 4 筆 (關聯)';
PRINT N'  - Guides: 4 筆 (美咲 / 美玲 / 阿諾 / 健一)';
PRINT N'  - GuideRoutes: 9 筆';
PRINT N'  - GuideReviews: 9 筆';
PRINT N'  - GuideFaqs: 9 筆';
