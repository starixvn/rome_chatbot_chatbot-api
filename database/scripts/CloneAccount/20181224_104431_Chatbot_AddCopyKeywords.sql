/*
	CreatedDate: 20181224@Kan
	Task: Add Copy keywords
	DB: DBBOT/bodb_ChatBot
*/

DELETE
FROM dbo.Resource
WHERE [Key] = 9 AND [Type] = 'Keyword' AND LanguageCode IN ('en-US','th-TH','zh-CN','zh-TW')
GO

INSERT INTO dbo.Resource ([Key], [Type], LanguageCode, [Value])
SELECT 9, 'Keyword', 'en-US', N'copy,new,create,clone,account,downline,acc,update'
UNION ALL
SELECT 9, 'Keyword', 'th-TH', N'ใหม่,คัดลอก,สร้างบัญชี'
UNION ALL
SELECT 9, 'Keyword', 'zh-CN', N'新增,新,复制,更新'
UNION ALL
SELECT 9, 'Keyword', 'zh-TW', N'新增,新,複製,更新'
GO

UPDATE dbo.Resource
SET [Value] = N'tạo,mới,sao chép,tài khoản,sẵn,tồn tại,bản sao,tao,moi,sao chep,saochep,taomoi,san,tontai,ton tai,ban sao,bansao'
WHERE [Key] = 9 AND [Type] = 'Keyword' AND LanguageCode = 'vi-VN'
GO