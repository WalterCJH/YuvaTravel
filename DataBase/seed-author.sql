-- ============================================================
-- 作者 (Author) Admin 權限 seed
--   1) 新增 FuncProgram 'Author' (掛 ArticleInfo 群組)
--   2) 授權 SystemG / AdminG
-- ============================================================
DECLARE @Now DATETIME = GETDATE();

IF NOT EXISTS (SELECT 1 FROM dbo.FuncPrograms WHERE FuncProgramId = 'Author')
BEGIN
    INSERT INTO dbo.FuncPrograms
        (FuncProgramId, FuncGroupId, [Name], Url, DisplaySeq, OnlyAdmin,
         IsActiveCreate, IsActiveEdit, IsActiveDelete, IsActiveDetails, IsActiveImport, IsActiveExport,
         IsChangeCreate, IsChangeEdit, IsChangeDelete, IsChangeDetails, IsChangeImport, IsChangeExport,
         AuthorizeLevel, CreateUserId, CreateTime)
    VALUES ('Author', 'ArticleInfo', N'作者', N'/Admin/Authors', 550, 0,
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
SELECT NEWID(), g.UserGroupId, 'Author', 1, 1, 1, 1, 0, 0, N'System', @Now
FROM TargetGroups g
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.UserGroupFuncPrograms x
    WHERE x.UserGroupId = g.UserGroupId AND x.FuncProgramId = 'Author'
);

PRINT N'Author Admin 權限 seed 完成';

-- ============================================================
-- (可選) 把現有 Guides 的 Name 自動產生對應 Author + 把現有 Articles 的 AuthorId 對應到 Author
-- 用法:依嚮導名稱在 Author 表建立同名作者,再用 Articles 既有的 Author NVARCHAR 欄位 (如還在)
--      做名稱配對。注意:Migration 已 drop 舊 Author 字串欄位,所以這步只能由你手動 UPDATE。
-- ============================================================

-- 範例:從 Guides 表建立同名 Author
DECLARE @AuthorSeq INT = 100;
INSERT INTO dbo.Authors (AuthorId, IsActive, Name, NameEn, Bio, GuideId, DisplaySeq, CreateUserId, CreateTime)
SELECT NEWID(), 1, g.Name, g.NameEn, g.Bio, g.GuideId,
       @AuthorSeq + (ROW_NUMBER() OVER (ORDER BY g.DisplaySeq) * 10),
       N'System', @Now
FROM dbo.Guides g
WHERE NOT EXISTS (SELECT 1 FROM dbo.Authors a WHERE a.GuideId = g.GuideId);

PRINT N'已為每位嚮導建立同名作者';

-- 文章關聯:依嚮導名稱對應 (如果有需要可解開註解,但通常要看你舊資料對應)
-- UPDATE a SET a.AuthorId = au.AuthorId
-- FROM dbo.Articles a
-- INNER JOIN dbo.Authors au ON au.Name = '舊作者字串值'
-- WHERE a.AuthorId IS NULL;
