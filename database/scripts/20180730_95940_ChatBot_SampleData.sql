/*
	CreatedDate: 20180730@Elodie
	Task: Prepare data to support ALPHA chat bot via letstalk
	DB: DBAGE/bodb_ChatBot
*/
USE bodb_ChatBot
GO

TRUNCATE TABLE dbo.UserInfo
TRUNCATE TABLE dbo.MessengerInfo
TRUNCATE TABLE dbo.[Function]
TRUNCATE TABLE dbo.Resource
TRUNCATE TABLE dbo.LastQueryUserMessage

----------UserInfo
GO
INSERT INTO dbo.UserInfo (CustId, SubAccId, Language)
SELECT TOP 100 custid, 0, 'en-US'
FROM bodb02.dbo.custInfo WITH (NOLOCK)
WHERE roleid = 4 AND Enabled = 1
UNION ALL
SELECT TOP 100 custid, 0, 'en-US'
FROM bodb02.dbo.custInfo WITH (NOLOCK)
WHERE roleid = 3 AND Enabled = 1
UNION ALL
SELECT TOP 100 custid, 0, 'en-US'
FROM bodb02.dbo.custInfo WITH (NOLOCK)
WHERE roleid = 2 AND Enabled = 1
UNION ALL
SELECT TOP 100 subaccid, custid, 'en-US'
FROM bodb02.dbo.custSub WITH (NOLOCK)

----------MessengerInfo
INSERT INTO dbo.MessengerInfo (UserId, MessengerUsername, MessengerType, Status, VerificationCode, VerificationCodeDateTime)
SELECT TOP 1 UserId, 'starixtest1', 1, 1, '123456', GETDATE()
FROM dbo.UserInfo WITH (NOLOCK)
WHERE UserId NOT IN (SELECT UserId FROM dbo.MessengerInfo WITH (NOLOCK))

INSERT INTO dbo.MessengerInfo (UserId, MessengerUsername, MessengerType, Status, VerificationCode, VerificationCodeDateTime)
SELECT TOP 1 UserId, 'starixtest2', 1, 1, '123456', GETDATE()
FROM dbo.UserInfo WITH (NOLOCK)
WHERE UserId NOT IN (SELECT UserId FROM dbo.MessengerInfo WITH (NOLOCK))

INSERT INTO dbo.MessengerInfo (UserId, MessengerUsername, MessengerType, Status, VerificationCode, VerificationCodeDateTime)
SELECT TOP 1 UserId, 'starixtest3', 1, 1, '123456', GETDATE()
FROM dbo.UserInfo WITH (NOLOCK)
WHERE UserId NOT IN (SELECT UserId FROM dbo.MessengerInfo WITH (NOLOCK))	

INSERT INTO dbo.MessengerInfo (UserId, MessengerUsername, MessengerType, Status, VerificationCode, VerificationCodeDateTime)
SELECT TOP 1 UserId, 'starixtest4', 1, 1, '123456', GETDATE()
FROM dbo.UserInfo WITH (NOLOCK)
WHERE UserId NOT IN (SELECT UserId FROM dbo.MessengerInfo WITH (NOLOCK))

INSERT INTO dbo.MessengerInfo (UserId, MessengerUsername, MessengerType, Status, VerificationCode, VerificationCodeDateTime)
SELECT TOP 1 UserId, 'starixtest5', 1, 1, '123456', GETDATE()
FROM dbo.UserInfo WITH (NOLOCK)
WHERE UserId NOT IN (SELECT UserId FROM dbo.MessengerInfo WITH (NOLOCK))

INSERT INTO dbo.MessengerInfo (UserId, MessengerUsername, MessengerType, Status, VerificationCode, VerificationCodeDateTime)
SELECT TOP 1 UserId, 'starixtest6', 1, 1, '123456', GETDATE()
FROM dbo.UserInfo WITH (NOLOCK)
WHERE UserId NOT IN (SELECT UserId FROM dbo.MessengerInfo WITH (NOLOCK))

INSERT INTO dbo.MessengerInfo (UserId, MessengerUsername, MessengerType, Status, VerificationCode, VerificationCodeDateTime)
SELECT TOP 1 UserId, 'starixtest7', 1, 1, '123456', GETDATE()
FROM dbo.UserInfo WITH (NOLOCK)
WHERE UserId NOT IN (SELECT UserId FROM dbo.MessengerInfo WITH (NOLOCK))

INSERT INTO dbo.MessengerInfo (UserId, MessengerUsername, MessengerType, Status, VerificationCode, VerificationCodeDateTime)
SELECT TOP 1 UserId, 'starixtest8', 1, 1, '123456', GETDATE()
FROM dbo.UserInfo WITH (NOLOCK)
WHERE UserId NOT IN (SELECT UserId FROM dbo.MessengerInfo WITH (NOLOCK))

INSERT INTO dbo.MessengerInfo (UserId, MessengerUsername, MessengerType, Status, VerificationCode, VerificationCodeDateTime)
SELECT TOP 1 UserId, 'starixtest9', 1, 1, '123456', GETDATE()
FROM dbo.UserInfo WITH (NOLOCK)
WHERE UserId NOT IN (SELECT UserId FROM dbo.MessengerInfo WITH (NOLOCK))

INSERT INTO dbo.MessengerInfo (UserId, MessengerUsername, MessengerType, Status, VerificationCode, VerificationCodeDateTime)
SELECT TOP 1 UserId, 'starixtest10', 1, 1, '123456', GETDATE()
FROM dbo.UserInfo WITH (NOLOCK)
WHERE UserId NOT IN (SELECT UserId FROM dbo.MessengerInfo WITH (NOLOCK))

----------[Function]
INSERT INTO dbo.[Function] (FunctionId, FunctionName, Type, DefaultCommand, Status)
SELECT 1, 'Outstanding', 1, 'os', 0
UNION ALL
SELECT 2, 'Today Win Loss', 1, 'wl', 0
UNION ALL
SELECT 3, 'Balance Info', 1, 'bal', 0
UNION ALL
SELECT 4, 'Yesterday Win Loss', 2, NULL, 0
UNION ALL
SELECT 5, 'Personal Message', 2, NULL, 0
UNION ALL				  
SELECT 6, 'Special Announcement', 2, NULL, 0
UNION ALL				  
SELECT 7, 'Normal Announcement', 2, NULL, 0
UNION ALL				  
SELECT 8, 'System Update', 2, NULL, 0

----------Resource
INSERT INTO dbo.Resource ( [Key], Type, LanguageCode, Value)
SELECT FunctionId, 'Function', 'en-US', FunctionName
FROM dbo.[Function] WITH (NOLOCK)

----------LastQueryUserMessage
INSERT INTO dbo.LastQueryUserMessage (UserId, FunctionId, MessageType, LastQueryTime)
SELECT uf.UserId, f.FunctionId, 1, GETDATE()
FROM dbo.MessengerInfo uf WITH (NOLOCK)
	CROSS APPLY dbo.[Function] f WITH (NOLOCK)
WHERE FunctionId BETWEEN 4 AND 8
