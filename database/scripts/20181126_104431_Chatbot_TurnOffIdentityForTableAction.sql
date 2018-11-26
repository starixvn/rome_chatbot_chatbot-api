/*
	CreatedDate: 20181126@Alvin
	Task: Turn
	DB: DBBOT/bodb_ChatBot
*/
USE bodb_ChatBot
GO

ALTER TABLE [dbo].[Action] ADD TempActionId INT NOT NULL DEFAULT 0
GO
UPDATE [dbo].[Action] SET TempActionId=ActionId
GO
ALTER TABLE [dbo].[Action] DROP CONSTRAINT PK_Action
GO
ALTER TABLE [dbo].[Action] DROP COLUMN ActionId
GO
EXEC sp_rename 'dbo.Action.TempActionId', 'ActionId', 'COLUMN';
GO
ALTER TABLE [dbo].[Action] ADD CONSTRAINT PK_Action PRIMARY KEY (ActionId);