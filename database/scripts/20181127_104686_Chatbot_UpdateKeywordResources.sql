/*
	CreatedDate: 20181127@Alvin
	Task: Insert Keyword Resources vi-VN, Update Keyword Resources zh-TW & zh-CN
	DB: DBBOT/bodb_ChatBot
*/
USE bodb_ChatBot
GO

UPDATE dbo.[Resource]
SET Value = N'輸贏,報告,今天,輸,贏'
WHERE [Key] = 2
	AND [Type] LIKE 'Keyword'
	AND LanguageCode LIKE 'zh-TW'

UPDATE dbo.[Resource]
SET Value = N'输赢,报告,今天,输,赢'
WHERE [Key] = 2
	AND [Type] LIKE 'Keyword'
	AND LanguageCode LIKE 'zh-CN'

UPDATE dbo.[Resource]
SET Value = N'餘額,資料,報告'
WHERE [Key] = 3
	AND [Type] LIKE 'Keyword'
	AND LanguageCode LIKE 'zh-TW'

UPDATE dbo.[Resource]
SET Value = N'余额,资料,报告'
WHERE [Key] = 3
	AND [Type] LIKE 'Keyword'
	AND LanguageCode LIKE 'zh-CN'

INSERT INTO dbo.Resource ([Key],Type,LanguageCode,Value)
SELECT 1, 'Keyword', 'vi-VN', N'chưa xử lý,báo cáo,đang diễn ra,chưa kết thúc,đang chạy,cược,bao cao,baocao,bc,chua xu ly,dang dien ra,chua ket thuc,dang chay,cuoc,kết quả,ket qua,kq'
UNION ALL
SELECT 2, 'Keyword', 'vi-VN', N'thắng,thua,báo cáo,hôm nay'
UNION ALL
SELECT 3, 'Keyword', 'vi-VN', N'số dư,tài khoản,báo cáo,hạn mức,còn lại'
