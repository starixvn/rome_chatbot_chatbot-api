/*
	CreatedDate: 20181116@Avlin
	Task: Add column [SelfdestructionPeriod] to table [UserInfo]
	DB: DBBOT/bodb_ChatBot
*/

USE bodb_ChatBot
GO

ALTER TABLE [dbo].[UserInfo] ADD SelfdestructionPeriod INT NULL;

GO
