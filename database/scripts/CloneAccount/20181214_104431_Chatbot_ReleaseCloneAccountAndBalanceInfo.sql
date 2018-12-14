/*
	CreatedDate: 20181214@Alvin
	Task: Release clone account & support view balance of top downline
	DB: DBBOT/bodb_ChatBot
*/


USE bodb_ChatBot
GO

------BALANCE----
UPDATE [dbo].[Action] SET IsEndAction = 0 WHERE ActionId=3;
UPDATE [dbo].[Action] SET ParentId = 3, [Status] = 0 WHERE ActionId IN (25,26);
DELETE [dbo].[Action] WHERE ActionId = 24;
DELETE [dbo].[Resource] WHERE [Key] = 24;

-----CLONE ACCOUNT----
UPDATE [dbo].[Function] SET [Status] = 0 WHERE FunctionId = 9;
UPDATE [dbo].[Action] SET [Status] = 0 WHERE ActionId BETWEEN 7 AND 23