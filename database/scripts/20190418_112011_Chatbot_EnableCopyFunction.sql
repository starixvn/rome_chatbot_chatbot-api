/*
	CreatedDate: 20190418@Elodie
	Task: Enable Copy Function
	DB: DBBOT/bodb_ChatBot
*/
USE bodb_ChatBot
GO

UPDATE [Function] WITH (ROWLOCK, UPDLOCK)
SET status = 0
WHERE FunctionId = 9