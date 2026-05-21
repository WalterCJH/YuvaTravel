-- ============================================================
-- 地區 (Region) Admin 權限 seed
--   1) 新增 FuncProgram 'Region' (掛 ArticleInfo 群組)
--   2) 授權 SystemG / AdminG
-- ============================================================
DECLARE @Now DATETIME = GETDATE();

IF NOT EXISTS (SELECT 1 FROM dbo.FuncPrograms WHERE FuncProgramId = 'Region')
BEGIN
    INSERT INTO dbo.FuncPrograms
        (FuncProgramId, FuncGroupId, [Name], Url, DisplaySeq, OnlyAdmin,
         IsActiveCreate, IsActiveEdit, IsActiveDelete, IsActiveDetails, IsActiveImport, IsActiveExport,
         IsChangeCreate, IsChangeEdit, IsChangeDelete, IsChangeDetails, IsChangeImport, IsChangeExport,
         AuthorizeLevel, CreateUserId, CreateTime)
    VALUES ('Region', 'ArticleInfo', N'地區', N'/Admin/Regions', 560, 0,
            1, 1, 1, 1, 0, 0,
            1, 1, 1, 1, 0, 0,
            10, N'System', @Now);
END

;WITH TargetGroups AS (
    SELECT UserGroupId FROM dbo.UserGroups WHERE UserGroupId IN ('SystemG','AdminG')
)
INSERT INTO dbo.UserGroupFuncPrograms
    (UserGroupFuncProgramId, UserGroupId, FuncProgramId,
     IsCreate, IsEdit, IsDelete, IsDetails, IsImport, IsExport,
     CreateUserId, CreateTime)
SELECT NEWID(), g.UserGroupId, 'Region', 1, 1, 1, 1, 0, 0, N'System', @Now
FROM TargetGroups g
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.UserGroupFuncPrograms x
    WHERE x.UserGroupId = g.UserGroupId AND x.FuncProgramId = 'Region'
);

PRINT N'Region Admin 權限 seed 完成';

-- ============================================================
-- (可選) 預設地區範例
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM dbo.Regions)
BEGIN
    INSERT INTO dbo.Regions (RegionId, IsActive, Code, [Name], NameEn, [Description], ImageUrl, DisplaySeq, CreateUserId, CreateTime) VALUES
    (NEWID(), 1, N'kyoto',    N'京都',     N'Kyoto',    N'千年古都,巷弄裡的時光感。', N'', 100, N'System', @Now),
    (NEWID(), 1, N'okinawa',  N'沖繩',     N'Okinawa',  N'陽光、海風,與三線琴的節奏。', N'', 200, N'System', @Now),
    (NEWID(), 1, N'tokyo',    N'東京',     N'Tokyo',    N'霓虹深夜與安靜的清晨。',     N'', 300, N'System', @Now),
    (NEWID(), 1, N'hokkaido', N'北海道',   N'Hokkaido', N'四季皆有獨自的安靜。',       N'', 400, N'System', @Now);

    PRINT N'已建立 4 筆預設地區';
END
