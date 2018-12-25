/*
	CreatedDate: 20181123@Alvin
	Task: Insert Clone Account Function Setting
	DB: DBBOT/bodb_ChatBot
*/
USE bodb_ChatBot
GO

-----Function-----
INSERT INTO dbo.[Function] ([FunctionId], [FunctionName], [Type], [DefaultCommand], [Status])
SELECT 9, 'Create new member from existing account', 3, '/copy', 1

-----FunctionSetting-----
INSERT INTO dbo.UserFunctionSetting ([UserId], [FunctionId], [Command], [Enabled])
SELECT UserId, 9, '/copy', 1 FROM UserInfo

-----Resource-----
INSERT INTO dbo.Resource ([Key], [Type], [LanguageCode], [Value])
SELECT 9, 'Function', 'en-US', 'Create new member from existing account'

-----Action-----
ALTER TABLE [dbo].[Action] ADD CustomerLevel TINYINT NOT NULL CONSTRAINT DF_Action_CustomerLevel DEFAULT 0
GO
ALTER TABLE [dbo].[Action] ADD BackToActionId INT NOT NULL CONSTRAINT DF_Action_BackToActionId DEFAULT 0
GO

----Delete OLD data if it exists----
DELETE FROM [dbo].[Action] WHERE ActionId BETWEEN 7 AND 23
DELETE FROM [dbo].[Resource] WHERE [Type] = 'Action' AND [Key] BETWEEN 7 AND 23

-----Insert Data-----
INSERT INTO [dbo].[Action] (ActionId, ActionName, Type, Command, DisplayOrder, FunctionId, ParentId, IsEndAction, Status, CustomerLevel, BackToActionId)
SELECT 7, 'Create new master from existing account', 1, '/copy', 40, 9, 0, 0, 1, 4, 0
UNION ALL
SELECT 8, 'Please type in an existing account.', 2, '', 0, 9, 7, 0, 1, 4, 0
UNION ALL
SELECT 9, 'Please type in a credit for new master.' + CHAR(13) + CHAR(10) + 'You can offer up to {0}.', 2, '', 0, 9, 8, 0, 1, 4, 0
UNION ALL
SELECT 10, 'Copy #actionid_8, credit #actionid_9', 1, '', 0, 9, 9, 1, 1, 4, 0
UNION ALL
SELECT 11, 'Reinput credit for copying #actionid_8', 1, '', 0, 9, 9, 0, 1, 4, 8
UNION ALL
SELECT 12, 'Create new agent from existing account', 1, '/copy', 70, 9, 0, 0, 1, 3, 0
UNION ALL
SELECT 13, 'Please type in an existing account.', 2, '', 0, 9, 12, 0, 1, 3, 0
UNION ALL
SELECT 14, 'Please type in a credit for new agent.' + CHAR(13) + CHAR(10) + 'You can offer up to {0}.', 2, '', 0, 9, 13, 0, 1, 3, 0
UNION ALL
SELECT 15, 'Please type in a max credit agent can offer member.' + CHAR(13) + CHAR(10) + 'Max member credit is up to {0}.', 2, '', 0, 9, 14, 0, 1, 3, 0
UNION ALL
SELECT 16, 'Copy #actionid_13, credit #actionid_14, max member credit #actionid_15', 1, '', 0, 9, 15, 1, 1, 3, 0
UNION ALL
SELECT 17, 'Reinput credit for copying #actionid_13', 1, '', 0, 9, 15, 0, 1, 3, 13
UNION ALL
SELECT 19, 'Create new member from existing account', 1, '/copy', 80, 9, 0, 0, 1, 2, 0
UNION ALL
SELECT 20, 'Please type in an existing account.', 2, '', 0, 9, 19, 0, 1, 2, 0
UNION ALL
SELECT 21, 'Please type in a credit for new member.' + CHAR(13) + CHAR(10) + 'You can offer up to {0}.', 2, '', 0, 9, 20, 0, 1, 2, 0
UNION ALL
SELECT 22, 'Copy #actionid_20, credit #actionid_21', 1, '', 0, 9, 21, 1, 1, 2, 0
UNION ALL
SELECT 23, 'Reinput credit for copying #actionid_20', 1, '', 0, 9, 21, 0, 1, 2, 20

INSERT INTO dbo.[Resource] ([Key], [Type], [LanguageCode], [Value])
SELECT ActionId, 'Action', 'en-US', ActionName FROM dbo.[Action]
WHERE ActionId BETWEEN 7 AND 23

-----New Balance Info-----
DELETE FROM [dbo].[Action] WHERE ActionId BETWEEN 24 AND 26
DELETE FROM [dbo].[Resource] WHERE [Type] = 'Action' AND [Key] BETWEEN 24 AND 26

INSERT INTO [dbo].[Action] (ActionId, ActionName, Type, Command, DisplayOrder, FunctionId, ParentId, IsEndAction, Status, CustomerLevel, BackToActionId)
SELECT 24, 'View Balance Info Report', 1, '/bal', 90, 3, 0, 0, 1, 0, 0
UNION ALL
SELECT 25, 'My Balance Info', 1, '', 100, 3, 24, 1, 1, 0, 0
UNION ALL
SELECT 26, 'Top 200 Balance Info', 1, '200', 110, 3, 24, 1, 1, 0, 0

INSERT INTO dbo.[Resource] ([Key], [Type], [LanguageCode], [Value])
SELECT ActionId, 'Action', 'en-US', ActionName FROM dbo.[Action]
WHERE ActionId BETWEEN 24 AND 26

-----Keywords for Clone Account---------
INSERT INTO dbo.Resource ([Key],Type,LanguageCode,Value)
SELECT 9, 'Keyword', 'en-US', N'copy,new,create,clone,account,downline,acc,update'
UNION ALL 
SELECT 9, 'Keyword', 'vi-VN', N'tạo,mới,sao chép,tài khoản,sẵn,tồn tại,bản sao'