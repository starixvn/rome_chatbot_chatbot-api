/*
	Created by: Alvin
	Date: 20181022
	Task: Insert LogAction for Chatbot
	DB: DBLOG / bodb_CustomerLog
*/
USE [bodb_CustomerLog]
GO

DECLARE @ActionID INT
	,@ActionNameEN NVARCHAR(256)
	,@ActionNameCH NVARCHAR(256)
	,@ActionNameCS NVARCHAR(256)
	,@ActionNameTH NVARCHAR(256)
	,@ActionNameVI NVARCHAR(256)
	,@SPName VARCHAR(50)
	,@IsInternal BIT
	,@ActionTypeID INT

SELECT @ActionID = 679
	,@ActionNameEN = 'Link Letstalk ID'
	,@ActionNameCH = N'鏈接Letstalk ID'
	,@ActionNameCS = N'链接Letstalk ID'
	,@ActionNameVI = N'Liên kết ID Letstalk'
	,@SPName = 'Chatbot_InsertUser'
	,@IsInternal = 0
	,@ActionTypeID = 4

INSERT INTO dbo.LogAction(ActionID, SPName, ActionTypeID, ActionNameEN, ActionNameCH, ActionNameCS, ActionNameJP, ActionNameTH, ActionNameKO, IsInternal, ActionNameVI)
VALUES (@ActionID, @SPName, @ActionTypeID, @ActionNameEN, @ActionNameCH, @ActionNameCS, @ActionNameEN, @ActionNameEN, @ActionNameEN, @IsInternal, @ActionNameVI)

SELECT @ActionID = 680
	,@ActionNameEN = 'Verify Letstalk ID'
	,@ActionNameCH = N'驗證Letstalk ID'
	,@ActionNameCS = N'验证Letstalk ID'
	,@ActionNameTH = N'ยืนยัน Letstalk ID'
	,@ActionNameVI = N'Xác thực ID Letstalk'
	,@SPName = 'Chatbot_UpdateMessengerAccountStatus'
	,@IsInternal = 0
	,@ActionTypeID = 4

INSERT INTO dbo.LogAction(ActionID, SPName, ActionTypeID, ActionNameEN, ActionNameCH, ActionNameCS, ActionNameJP, ActionNameTH, ActionNameKO, IsInternal, ActionNameVI)
VALUES (@ActionID, @SPName, @ActionTypeID, @ActionNameEN, @ActionNameCH, @ActionNameCS, @ActionNameEN, @ActionNameTH, @ActionNameEN, @IsInternal, @ActionNameVI)

SELECT @ActionID = 681
	,@ActionNameEN = 'Update Chatbot''s Language'
	,@ActionNameCH = N'更新 Chatbot 語言'
	,@ActionNameCS = N'更新 Chatbot 语言'
	,@ActionNameTH = N'อัปเดตภาษา Chatbot'
	,@ActionNameVI = N'Cập nhật ngôn ngữ Chatbot'
	,@SPName = 'Chatbot_UpdateUserLanguage'
	,@IsInternal = 0
	,@ActionTypeID = 4

INSERT INTO dbo.LogAction(ActionID, SPName, ActionTypeID, ActionNameEN, ActionNameCH, ActionNameCS, ActionNameJP, ActionNameTH, ActionNameKO, IsInternal, ActionNameVI)
VALUES (@ActionID, @SPName, @ActionTypeID, @ActionNameEN, @ActionNameCH, @ActionNameCS, @ActionNameEN, @ActionNameTH, @ActionNameEN, @IsInternal, @ActionNameVI)

SELECT @ActionID = 682
	,@ActionNameEN = 'Update Chatbot''s Function'
	,@ActionNameCH = N'更新 Chatbot 功能'
	,@ActionNameCS = N'更新 Chatbot 功能'
	,@ActionNameTH = N'อัปเดตฟังชั่น Chatbot'
	,@ActionNameVI = N'Cập nhật chức năng Chatbot'
	,@SPName = 'Chatbot_UpdateUserFunctions'
	,@IsInternal = 0
	,@ActionTypeID = 4

INSERT INTO dbo.LogAction(ActionID, SPName, ActionTypeID, ActionNameEN, ActionNameCH, ActionNameCS, ActionNameJP, ActionNameTH, ActionNameKO, IsInternal, ActionNameVI)
VALUES (@ActionID, @SPName, @ActionTypeID, @ActionNameEN, @ActionNameCH, @ActionNameCS, @ActionNameEN, @ActionNameTH, @ActionNameEN, @IsInternal, @ActionNameVI)

SELECT @ActionID = 683
	,@ActionNameEN = 'Unlink Letstalk ID'
	,@ActionNameCH = N'取消鏈接 Letstalk ID'
	,@ActionNameCS = N'取消链接 Letstalk ID'
	,@ActionNameTH = N'ยกเลิก link Lestalk ID'
	,@ActionNameVI = N'Hủy liên kết ID Letstalk'
	,@SPName = 'Chatbot_UpdateMessengerAccountStatus'
	,@IsInternal = 0
	,@ActionTypeID = 4

INSERT INTO dbo.LogAction(ActionID, SPName, ActionTypeID, ActionNameEN, ActionNameCH, ActionNameCS, ActionNameJP, ActionNameTH, ActionNameKO, IsInternal, ActionNameVI)
VALUES (@ActionID, @SPName, @ActionTypeID, @ActionNameEN, @ActionNameCH, @ActionNameCS, @ActionNameEN, @ActionNameTH, @ActionNameEN, @IsInternal, @ActionNameVI)
