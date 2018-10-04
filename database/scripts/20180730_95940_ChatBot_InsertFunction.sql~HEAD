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
SELECT 1, 'Outstanding', 1, '/os', 0
UNION ALL
SELECT 2, 'Today Win Loss', 1, '/wl', 0
UNION ALL
SELECT 3, 'Balance Info', 1, '/bal', 0
UNION ALL
SELECT 4, 'Yesterday Win Loss (Daily sent at 3:00 AM GMT-4)', 2, NULL, 0
UNION ALL
SELECT 5, 'Personal Message', 2, NULL, 0
UNION ALL				  
SELECT 6, 'Special Announcement', 2, NULL, 0
UNION ALL				  
SELECT 7, 'Normal Announcement', 2, NULL, 0
UNION ALL				  
SELECT 8, 'System Announcement', 2, NULL, 0

----------Resource
--en-US
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT FunctionId, 'Function', 'en-US', FunctionName
FROM dbo.[Function] WITH (NOLOCK)

--ja-JP
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 1, 'Function', 'ja-JP', N'未払い'


--ko-KR
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 5, 'Function', 'ko-KR', N'개인 메세지'
UNION ALL
SELECT 6, 'Function', 'ko-KR', N'특별 공지사항'
UNION ALL
SELECT 7, 'Function', 'ko-KR', N'일반 공지 사항'
UNION ALL
SELECT 2, 'Function', 'ko-KR', N'오늘의 승리/패배'
UNION ALL
SELECT 1, 'Function', 'ko-KR', N'대기 포인트'

--th-TH
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 5, 'Function', 'th-TH', N'ข้อความส่วนตัว'
UNION ALL			   
SELECT 6, 'Function', 'th-TH', N'ข้อความ เฉพาะ'
UNION ALL			   
SELECT 7, 'Function', 'th-TH', N'ประกาศธรรมดา'
UNION ALL			   
SELECT 2, 'Function', 'th-TH', N'ยอดได้เสียวันนี้'
UNION ALL			   
SELECT 1, 'Function', 'th-TH', N'การเดิมพันที่ยังไม่สิ้นสุด'

--vi-VN
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 5, 'Function', 'vi-VN', N'Tin nhắn cá nhân'
UNION ALL			  
SELECT 6, 'Function', 'vi-VN', N'Thông báo đăc biệt'
UNION ALL			  
SELECT 7, 'Function', 'vi-VN', N'Thông báo chung'
UNION ALL			   
SELECT 2, 'Function', 'vi-VN', N'Thắng thua trong ngày'
UNION ALL			  
SELECT 1, 'Function', 'vi-VN', N'Tiền chưa xử lý'

--zh-CN
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 5, 'Function', 'zh-CN', N'个人讯息'
UNION ALL			  
SELECT 6, 'Function', 'zh-CN', N'特殊公告'
UNION ALL			  
SELECT 7, 'Function', 'zh-CN', N'一般公告'
UNION ALL			  
SELECT 2, 'Function', 'zh-CN', N'今日输赢'
UNION ALL			  
SELECT 1, 'Function', 'zh-CN', N'未完成交易'

--zh-TW
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT 5, 'Function', 'zh-TW', N'個人訊息'
UNION ALL			 
SELECT 6, 'Function', 'zh-TW', N'特殊公告'
UNION ALL			  
SELECT 7, 'Function', 'zh-TW', N'一般公告'
UNION ALL			 
SELECT 2, 'Function', 'zh-TW', N'今日輸贏'
UNION ALL			  
SELECT 1, 'Function', 'zh-TW', N'未完成交易'

INSERT INTO Setting (SettingName, Value)
SELECT 'LastQueryPersonalMessage', CONVERT(VARCHAR(200), GETDATE(),21)
UNION ALL
SELECT 'LastQuerySpecialAnnouncement', CONVERT(VARCHAR(200), GETDATE(),21)
UNION ALL
SELECT 'LastQueryNormalAnnouncement', CONVERT(VARCHAR(200), GETDATE(),21)
UNION ALL
SELECT 'LastQuerySystemAnnouncement', CONVERT(VARCHAR(200), GETDATE(),21)

