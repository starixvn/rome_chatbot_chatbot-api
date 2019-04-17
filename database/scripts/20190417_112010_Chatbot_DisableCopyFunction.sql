/*
	CreatedDate: 20190417@Ricky
	Task: Disable Copy Function
	DB: DBBOT/bodb_ChatBot
*/

UPDATE [bodb_ChatBot].[dbo].[Function] WITH (ROWLOCK, UPDLOCK)
SET status = 2
WHERE FunctionId = 9