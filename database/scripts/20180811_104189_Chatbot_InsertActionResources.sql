/*
	CreatedDate: 20180811@Alvin
	Task: Insert resource for Action
	DB: DBAGE/bodb_ChatBot
*/

USE bodb_ChatBot
GO

--en-US
INSERT INTO dbo.Resource ([Key], Type, LanguageCode,Value)
SELECT ActionId, 'Action', 'en-US', ActionName 
FROM dbo.[Action] WITH (NOLOCK)