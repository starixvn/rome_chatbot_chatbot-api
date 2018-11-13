/*
	CreatedDate: 20181025@Alvin
	Task: Insert Function Outstanding Detail
	DB: DBAGE/bodb_ChatBot
*/
USE bodb_ChatBot
GO

----------[Function]
INSERT INTO dbo.[Function] (FunctionId, FunctionName, Type, DefaultCommand, Status)
VALUES (9, 'Outstanding Detail', 1, '/osd', 0)

----------[Resource]
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 9, 'Function', 'en-US', 'Outstanding Detail'