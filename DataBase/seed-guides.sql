-- ============================================================
-- Guide 系列 Admin 權限 seed
-- 執行內容：
--   1) 新增 FuncGroup「嚮導資訊」(GuideInfo)
--   2) 新增 4 個 FuncProgram (Guide / GuideRoute / GuideReview / GuideFaq)
--   3) 授權 SystemG / AdminG 兩個群組擁有完整權限
--
-- 執行前請先確認資料庫中尚未有 GuideInfo / Guide / GuideRoute /
-- GuideReview / GuideFaq 的 FuncGroup / FuncProgram。
-- 若有重複可先用以下查詢確認:
--   SELECT * FROM FuncGroups WHERE FuncGroupId = 'GuideInfo';
--   SELECT * FROM FuncPrograms WHERE FuncProgramId IN ('Guide','GuideRoute','GuideReview','GuideFaq');
-- ============================================================

DECLARE @Now DATETIME = GETDATE();

-- 1) FuncGroup
IF NOT EXISTS (SELECT 1 FROM dbo.FuncGroups WHERE FuncGroupId = 'GuideInfo')
BEGIN
    INSERT INTO dbo.FuncGroups (FuncGroupId, [Name], IconClass, DisplaySeq, CreateUserId, CreateTime)
    VALUES ('GuideInfo', N'嚮導資訊', N'fas fa-user-friends', 35, N'System', @Now);
END

-- 2) FuncPrograms
IF NOT EXISTS (SELECT 1 FROM dbo.FuncPrograms WHERE FuncProgramId = 'Guide')
BEGIN
    INSERT INTO dbo.FuncPrograms
        (FuncProgramId, FuncGroupId, [Name], Url, DisplaySeq, OnlyAdmin,
         IsActiveCreate, IsActiveEdit, IsActiveDelete, IsActiveDetails, IsActiveImport, IsActiveExport,
         IsChangeCreate, IsChangeEdit, IsChangeDelete, IsChangeDetails, IsChangeImport, IsChangeExport,
         AuthorizeLevel, CreateUserId, CreateTime)
    VALUES ('Guide', 'GuideInfo', N'嚮導', N'/Admin/Guides', 100, 0,
            1, 1, 1, 1, 1, 0,
            1, 1, 1, 1, 1, 0,
            10, N'System', @Now);
END

IF NOT EXISTS (SELECT 1 FROM dbo.FuncPrograms WHERE FuncProgramId = 'GuideRoute')
BEGIN
    INSERT INTO dbo.FuncPrograms
        (FuncProgramId, FuncGroupId, [Name], Url, DisplaySeq, OnlyAdmin,
         IsActiveCreate, IsActiveEdit, IsActiveDelete, IsActiveDetails, IsActiveImport, IsActiveExport,
         IsChangeCreate, IsChangeEdit, IsChangeDelete, IsChangeDetails, IsChangeImport, IsChangeExport,
         AuthorizeLevel, CreateUserId, CreateTime)
    VALUES ('GuideRoute', 'GuideInfo', N'嚮導路線', N'/Admin/GuideRoutes', 200, 0,
            1, 1, 1, 0, 0, 0,
            1, 1, 1, 0, 0, 0,
            10, N'System', @Now);
END

IF NOT EXISTS (SELECT 1 FROM dbo.FuncPrograms WHERE FuncProgramId = 'GuideReview')
BEGIN
    INSERT INTO dbo.FuncPrograms
        (FuncProgramId, FuncGroupId, [Name], Url, DisplaySeq, OnlyAdmin,
         IsActiveCreate, IsActiveEdit, IsActiveDelete, IsActiveDetails, IsActiveImport, IsActiveExport,
         IsChangeCreate, IsChangeEdit, IsChangeDelete, IsChangeDetails, IsChangeImport, IsChangeExport,
         AuthorizeLevel, CreateUserId, CreateTime)
    VALUES ('GuideReview', 'GuideInfo', N'嚮導評論', N'/Admin/GuideReviews', 300, 0,
            1, 1, 1, 0, 0, 0,
            1, 1, 1, 0, 0, 0,
            10, N'System', @Now);
END

IF NOT EXISTS (SELECT 1 FROM dbo.FuncPrograms WHERE FuncProgramId = 'GuideFaq')
BEGIN
    INSERT INTO dbo.FuncPrograms
        (FuncProgramId, FuncGroupId, [Name], Url, DisplaySeq, OnlyAdmin,
         IsActiveCreate, IsActiveEdit, IsActiveDelete, IsActiveDetails, IsActiveImport, IsActiveExport,
         IsChangeCreate, IsChangeEdit, IsChangeDelete, IsChangeDetails, IsChangeImport, IsChangeExport,
         AuthorizeLevel, CreateUserId, CreateTime)
    VALUES ('GuideFaq', 'GuideInfo', N'嚮導 FAQ', N'/Admin/GuideFaqs', 400, 0,
            1, 1, 1, 0, 0, 0,
            1, 1, 1, 0, 0, 0,
            10, N'System', @Now);
END

-- 3) UserGroupFuncPrograms (SystemG + AdminG)
;WITH ToGrant AS (
    SELECT FuncProgramId, IsImport FROM (VALUES
        ('Guide', 1), ('GuideRoute', 0), ('GuideReview', 0), ('GuideFaq', 0)
    ) v(FuncProgramId, IsImport)
), TargetGroups AS (
    SELECT UserGroupId FROM dbo.UserGroups WHERE UserGroupId IN ('SystemG','AdminG')
)
INSERT INTO dbo.UserGroupFuncPrograms
    (UserGroupFuncProgramId, UserGroupId, FuncProgramId,
     IsCreate, IsEdit, IsDelete, IsDetails, IsImport, IsExport,
     CreateUserId, CreateTime)
SELECT
    NEWID(), g.UserGroupId, p.FuncProgramId,
    1, 1, 1, 1, p.IsImport, 0,
    N'System', @Now
FROM TargetGroups g
CROSS JOIN ToGrant p
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.UserGroupFuncPrograms x
    WHERE x.UserGroupId = g.UserGroupId AND x.FuncProgramId = p.FuncProgramId
);

PRINT 'Guide 系列 Admin 權限 seed 完成。';
