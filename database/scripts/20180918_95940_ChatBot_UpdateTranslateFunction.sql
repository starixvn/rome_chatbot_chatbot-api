/*
	CreatedDate: 20180919@Elodie
	Task: Add translation 
	DB: DBAGE/bodb_ChatBot
*/
USE bodb_ChatBot
GO

----------Resource
--th-TH
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 4, 'Function', 'th-TH', N'ยอดชนะ/เสียของเมื่อวาน'
UNION ALL			   
SELECT 8, 'Function', 'th-TH', N'ประกาศระบบ'

--zh-CN
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 4, 'Function', 'zh-CN', N'昨日输赢 （每日发送于凌晨03:00 (GMT-4))'
UNION ALL			  
SELECT 8, 'Function', 'zh-CN', N'系统公告'

--zh-TW
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 4, 'Function', 'zh-TW', N'昨日輸贏 （每日發送於淩晨03:00 (GMT-4))'
UNION ALL			 
SELECT 8, 'Function', 'zh-TW', N'系統公告'

