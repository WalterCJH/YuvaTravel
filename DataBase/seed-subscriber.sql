-- ============================================================
-- 電子報訂閱 (Subscriber) Admin 權限 seed
--   1) 新增 FuncProgram 'Subscriber' (掛 WebPageInfo 群組)
--   2) 授權 SystemG / AdminG
-- ============================================================
DECLARE @Now DATETIME = GETDATE();

IF NOT EXISTS (SELECT 1 FROM dbo.FuncPrograms WHERE FuncProgramId = 'Subscriber')
BEGIN
    INSERT INTO dbo.FuncPrograms
        (FuncProgramId, FuncGroupId, [Name], Url, DisplaySeq, OnlyAdmin,
         IsActiveCreate, IsActiveEdit, IsActiveDelete, IsActiveDetails, IsActiveImport, IsActiveExport,
         IsChangeCreate, IsChangeEdit, IsChangeDelete, IsChangeDetails, IsChangeImport, IsChangeExport,
         AuthorizeLevel, CreateUserId, CreateTime)
    VALUES ('Subscriber', 'WebPageInfo', N'電子報訂閱', N'/Admin/Subscribers', 700, 0,
            0, 1, 1, 1, 0, 0,   -- IsActive*:不開新增 (前台訂閱才能新增),開 Edit (切啟用) / Delete / Details
            0, 1, 1, 1, 0, 0,
            10, N'System', @Now);
END

;WITH TargetGroups AS (
    SELECT UserGroupId FROM dbo.UserGroups WHERE UserGroupId IN ('SystemG','AdminG')
)
INSERT INTO dbo.UserGroupFuncPrograms
    (UserGroupFuncProgramId, UserGroupId, FuncProgramId,
     IsCreate, IsEdit, IsDelete, IsDetails, IsImport, IsExport,
     CreateUserId, CreateTime)
SELECT NEWID(), g.UserGroupId, 'Subscriber', 0, 1, 1, 1, 0, 0, N'System', @Now
FROM TargetGroups g
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.UserGroupFuncPrograms x
    WHERE x.UserGroupId = g.UserGroupId AND x.FuncProgramId = 'Subscriber'
);

PRINT N'Subscriber Admin 權限 seed 完成';
