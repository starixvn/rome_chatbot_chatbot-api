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
SELECT UserId, 9, '/copy', 0 FROM UserInfo

-----Resource-----
INSERT INTO dbo.Resource ([Key], [Type], [LanguageCode], [Value])
SELECT 9, 'Function', 'en-US', 'Create new member from existing account'


-----Action-----ALTER TABLE [dbo].[Action] ADD CustomerLevel TINYINT NOT NULL DEFAULT 0GO-----Insert Data-----INSERT INTO [dbo].[Action] (ActionId, ActionName, Type, Command, DisplayOrder, FunctionId, ParentId, IsEndAction, Status, CustomerLevel)SELECT 7, 'Create new master from existing Account', 1, '/copy', 40, 9, 0, 0, 1, 4UNION ALLSELECT 8, 'From which master do you want to copy setting?', 2, '', 0, 9, 7, 0, 1, 4UNION ALLSELECT 9, 'How much credit do you want to offer your new master?' + CHAR(13) + CHAR(10) + 'You can offer up to {0}.', 2, '', 0, 9, 8, 0, 1, 4