/*
	CreatedDate: 20180919@Elodie
	Task: Add translation 
	DB: DBAGE/bodb_ChatBot
*/
USE bodb_ChatBot
GO

----------Resource
--ko-KR
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 3, 'Function', 'ko-KR', N'잔액정보'

--th-TH
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 3, 'Function', 'th-TH', N'ข้อมูลยอดเงินคงเหลือ'

--vi-VN
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 3, 'Function', 'vi-VN', N'Tài Khoản'

--zh-CN
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 3, 'Function', 'zh-CN', N'余额资料'

--zh-TW
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 3, 'Function', 'zh-TW', N'餘額資料'

