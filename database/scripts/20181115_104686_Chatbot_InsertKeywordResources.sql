/*
	CreatedDate: 20181115@Alvin
	Task: Insert Keyword Resources
	DB: DBBOT/bodb_ChatBot
*/

USE bodb_ChatBot
GO

--en-US
INSERT INTO dbo.Resource ([Key],Type,LanguageCode,Value)
SELECT 1, 'Keyword', 'en-US', 'outstanding,out,os,ostd,report,view,running'
UNION ALL
SELECT 2, 'Keyword', 'en-US', 'today,win,loss,wl,twl,report'
UNION ALL
SELECT 3, 'Keyword', 'en-US', 'balance,info,report,bal,bl'