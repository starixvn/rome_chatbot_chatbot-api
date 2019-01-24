USE bodb_ChatBot
GO

ALTER TABLE [dbo].[UserInfo]
	ALTER COLUMN [Language] varchar(7)
GO

ALTER TABLE [dbo].[Resource]
	DROP CONSTRAINT PK_Resource
GO

ALTER TABLE [dbo].[Resource]
    ALTER COLUMN [LanguageCode] varchar(7) NOT NULL;
GO

ALTER TABLE [dbo].[Resource]
	ADD CONSTRAINT PK_Resource PRIMARY KEY CLUSTERED ([KEY],[Type], [LanguageCode])
GO 

INSERT [dbo].[Resource] (
	[Key]
	,[Type]
	,[LanguageCode]
	,[Value]
	)
SELECT [Key]
	,[Type]
	,'zh-Hans'
	,[Value]
FROM [dbo].[Resource]
WHERE LanguageCode LIKE 'zh-CN'
