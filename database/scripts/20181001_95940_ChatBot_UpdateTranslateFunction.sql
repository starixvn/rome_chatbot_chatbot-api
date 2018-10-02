/*
	CreatedDate: 20181001@Elodie
	Task: Add translation 
	DB: DBAGE/bodb_ChatBot
*/
USE bodb_ChatBot
GO

----------Resource
--vi-VN
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 8, 'Function', 'vi-VN', N'Thông báo hệ thống'
UNION ALL
SELECT 4, 'Function', 'vi-VN', N'Thắng thua hôm qua (gửi hàng ngày lúc 3:00 AM GMT-4)'

