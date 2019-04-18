/*
	CreatedDate: 20190418@Elodie
	Task: Disable Copy Function for user
	DB: DBBOT/bodb_ChatBot
*/
USE bodb_ChatBot
GO

--1016 affected rows
UPDATE dbo.UserFunctionSetting WITH (ROWLOCK, UPDLOCK)
SET Enabled = 0
WHERE FunctionId = 9