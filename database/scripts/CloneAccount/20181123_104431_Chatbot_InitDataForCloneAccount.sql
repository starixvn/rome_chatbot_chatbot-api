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

-----Action-----
ALTER TABLE [dbo].[Action] ADD CustomerLevel TINYINT NOT NULL DEFAULT 0
GO
ALTER TABLE [dbo].[Action] ADD BackToActionId INT NOT NULL DEFAULT 0
GO

----Delete OLD data if it exists----
DELETE FROM [dbo].[Action] WHERE ActionId BETWEEN 7 AND 23
DELETE FROM [dbo].[Resource] WHERE [Type] = 'Action' AND [Key] BETWEEN 7 AND 23

-----Insert Data-----
INSERT INTO [dbo].[Action] (ActionId, ActionName, Type, Command, DisplayOrder, FunctionId, ParentId, IsEndAction, Status, CustomerLevel, BackToActionId)
SELECT 7, 'Create new master from existing account', 1, '/copy', 40, 9, 0, 0, 1, 4, 0
UNION ALL
SELECT 8, 'From which master do you want to copy setting?', 2, '', 0, 9, 7, 0, 1, 4, 0
UNION ALL
SELECT 9, 'How much credit do you want to offer your new master?' + CHAR(13) + CHAR(10) + 'You can offer up to {0}.', 2, '', 0, 9, 8, 0, 1, 4, 0
UNION ALL
SELECT 10, 'Create new master from #actionid_8, credit #actionid_9', 1, '', 0, 9, 9, 1, 1, 4, 0
UNION ALL
SELECT 11, 'Reinput credit for creating new master from #actionid_8', 1, '', 0, 9, 9, 0, 1, 4, 8
UNION ALL
SELECT 12, 'Create new agent from existing account', 1, '/copy', 70, 9, 0, 0, 1, 3, 0
UNION ALL
SELECT 13, 'From which agent do you want to copy setting?', 2, '', 0, 9, 12, 0, 1, 3, 0
UNION ALL
SELECT 14, 'How much credit do you want to offer your new agent?' + CHAR(13) + CHAR(10) + 'You can offer up to {0}.', 2, '', 0, 9, 13, 0, 1, 3, 0
UNION ALL
SELECT 15, 'Please input the max credit that the new agent can offer to his/her members?' + CHAR(13) + CHAR(10) + 'You can offer up to {0}.', 2, '', 0, 9, 14, 0, 1, 3, 0
UNION ALL
SELECT 16, 'Create new agent from #actionid_13, credit #actionid_14, max member credit #actionid_15', 1, '', 0, 9, 15, 1, 1, 3, 0
UNION ALL
SELECT 17, 'Reinput credit for creating new agent from #actionid_13', 1, '', 0, 9, 15, 0, 1, 3, 13
UNION ALL
SELECT 18, 'Reinput max member credit for creating new agent from #actionid_13 ', 1, '', 0, 9, 15, 0, 1, 3, 14
UNION ALL
SELECT 19, 'Create new member from existing account', 1, '/copy', 80, 9, 0, 0, 1, 2, 0
UNION ALL
SELECT 20, 'From which member do you want to copy setting?', 2, '', 0, 9, 19, 0, 1, 2, 0
UNION ALL
SELECT 21, 'How much credit do you want to offer your new member?' + CHAR(13) + CHAR(10) + 'You can offer up to {0}.', 2, '', 0, 9, 20, 0, 1, 2, 0
UNION ALL
SELECT 22, 'Create new member from #actionid_20, credit #actionid_21', 1, '', 0, 9, 21, 1, 1, 2, 0
UNION ALL
SELECT 23, 'Reinput credit for creating new member from #actionid_20', 1, '', 0, 9, 21, 0, 1, 2, 20

INSERT INTO dbo.[Resource] ([Key], [Type], [LanguageCode], [Value])
SELECT ActionId, 'Action', 'en-US', ActionName FROM dbo.[Action]
WHERE ActionId BETWEEN 7 AND 23