/*
	CreatedDate: 20181120@Kan
	Task: Insert multiple languages into Resource table
	DB: DBBOT/bodb_ChatBot
*/

USE [bodb_ChatBot]
GO

--zh-CN
INSERT INTO dbo.Resource([Key], [Type], LanguageCode, [Value])
SELECT 1, 'Action', 'zh-CN', N'查看未结束交易报告'
UNION ALL
SELECT 2, 'Action', 'zh-CN', N'查看今日输赢'
UNION ALL
SELECT 3, 'Action', 'zh-CN', N'查看余额资料'
UNION ALL
SELECT 4, 'Action', 'zh-CN', N'特定直属下线'
UNION ALL
SELECT 5, 'Action', 'zh-CN', N'前50未结束交易'
UNION ALL
SELECT 6, 'Action', 'zh-CN', N'请输入用户名'

--zh-TW
INSERT INTO dbo.Resource([Key], [Type], LanguageCode, [Value])
SELECT 1, 'Action', 'zh-TW', N'查看未結束交易報告'
UNION ALL
SELECT 2, 'Action', 'zh-TW', N'查看今日輸贏'
UNION ALL
SELECT 3, 'Action', 'zh-TW', N'查看余額資料'
UNION ALL
SELECT 4, 'Action', 'zh-TW', N'特定直屬下線'
UNION ALL
SELECT 5, 'Action', 'zh-TW', N'前50未結束交易'
UNION ALL
SELECT 6, 'Action', 'zh-TW', N'請輸入用戶名'

--th-TH
INSERT INTO dbo.Resource([Key], [Type], LanguageCode, [Value])
SELECT 1, 'Action', 'th-TH', N'ดูรายงานยอดคงค้าง'
UNION ALL
SELECT 2, 'Action', 'th-TH', N'ดูยอดได้เสียประจำวัน'
UNION ALL
SELECT 3, 'Action', 'th-TH', N'ดูข้อมูลยอดคงเหลือ'
UNION ALL
SELECT 4, 'Action', 'th-TH', N'เจาะจงดาวน์ไลน์โดยตรง'
UNION ALL
SELECT 5, 'Action', 'th-TH', N'ยอดคงค้าง 50 รายการแรก'
UNION ALL
SELECT 6, 'Action', 'th-TH', N'โปรดป้อนชื่อผู้ใช้'
