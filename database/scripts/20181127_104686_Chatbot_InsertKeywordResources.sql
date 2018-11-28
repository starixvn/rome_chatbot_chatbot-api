/*
	CreatedDate: 20181127@Alvin
	Task: Insert Keyword Resources
	DB: DBBOT/bodb_ChatBot
*/

USE bodb_ChatBot
GO

--en-US
INSERT INTO dbo.Resource ([Key],Type,LanguageCode,Value)
SELECT 1, 'Keyword', 'zh-TW', N'交易,未結束,報告'
UNION ALL
SELECT 1, 'Keyword', 'zh-CN', N'交易,未结束,报告'
UNION ALL
SELECT 1, 'Keyword', 'th-TH', N'การเดิมพันที่ยังไม่สิ้นสุดของ'
UNION ALL
SELECT 2, 'Keyword', 'zh-TW', N'輸贏，報告，今天，輸，贏'
UNION ALL
SELECT 2, 'Keyword', 'zh-CN', N'输赢，报告，今天，输，赢'
UNION ALL
SELECT 2, 'Keyword', 'th-TH', N'ชนะ,แพ้'
UNION ALL
SELECT 3, 'Keyword', 'zh-TW', N'餘額，資料，報告'
UNION ALL
SELECT 3, 'Keyword', 'zh-CN', N'余额，资料，报告'
UNION ALL
SELECT 3, 'Keyword', 'th-TH', N'ข้อมูล,ยอดเงินคงเหลือ'