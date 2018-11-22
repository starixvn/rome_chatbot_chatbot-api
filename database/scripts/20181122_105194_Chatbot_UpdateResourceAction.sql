/*
	CreatedDate: 20181122@Elodie
	Task: Update data into Action table [Redmine:#105194]
	DB: DBBOT/bodb_ChatBot
    Column explanation:
*/

USE [bodb_ChatBot]
GO

UPDATE a
SET a.ActionName = 'Top 200 Outstanding',
	a.Command = 200,
	a.DisplayOrder = 50
FROM Action a WITH (ROWLOCK)
WHERE a.ActionId = 5

UPDATE r
SET r.Value = CASE	WHEN r.LanguageCode = 'en-US' THEN N'Top 200 Outstanding'
					WHEN r.LanguageCode = 'th-TH' THEN N'ยอดคงค้าง 200 รายการแรก'
					WHEN r.LanguageCode = 'zh-CN' THEN N'前200未结束交易'
					WHEN r.LanguageCode = 'zh-TW' THEN N'前200未結束交易' END
FROM Resource r WITH (ROWLOCK)
WHERE r.[Key] = 5 AND r.Type = 'Action'