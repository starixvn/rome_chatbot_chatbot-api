/*
	CreatedDate: 20180730@Elodie
	Task: Prepare data to support ALPHA chat bot via letstalk
	DB: DBAGE/bodb_ChatBot
*/
USE bodb_ChatBot
GO

--TRUNCATE TABLE dbo.[Function]
--TRUNCATE TABLE dbo.Resource

----------[Function]
INSERT INTO dbo.[Function] (FunctionId, FunctionName, Type, DefaultCommand, Status)
SELECT 1, 'Outstanding', 1, 'os', 0
UNION ALL
SELECT 2, 'Today Win Loss', 1, 'wl', 0
UNION ALL
SELECT 3, 'Balance Info', 1, 'bal', 0
UNION ALL
SELECT 4, 'Yesterday Win Loss', 2, NULL, 0
UNION ALL
SELECT 5, 'Personal Message', 2, NULL, 0
UNION ALL				  
SELECT 6, 'Special Announcement', 2, NULL, 0
UNION ALL				  
SELECT 7, 'Normal Announcement', 2, NULL, 0
UNION ALL				  
SELECT 8, 'System Announcement', 2, NULL, 0

----------Resource
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT FunctionId, 'Function', 'en-US', FunctionName
FROM dbo.[Function] WITH (NOLOCK)

