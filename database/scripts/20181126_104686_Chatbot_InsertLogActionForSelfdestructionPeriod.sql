/*
	Created by: Alvin
	Date: 20181126
	Task: Insert LogAction for Chatbot Self-destruction period
	DB: DBLOG / bodb_CustomerLog
*/

USE [bodb_CustomerLog]
GO

DECLARE @ActionID INT
	,@ActionNameEN NVARCHAR(256)
	,@SPName VARCHAR(50)
	,@IsInternal BIT
	,@ActionTypeID INT

SELECT @ActionID = 684
	,@ActionNameEN = 'Update Chatbot''s Self-destruction Period'
	,@SPName = 'Chatbot_UpdateSelfdestructionPeriod'
	,@IsInternal = 0
	,@ActionTypeID = 4

INSERT INTO dbo.LogAction(ActionID, SPName, ActionTypeID, ActionNameEN, ActionNameCH, ActionNameCS, ActionNameJP, ActionNameTH, ActionNameKO, IsInternal, ActionNameVI)
VALUES (@ActionID, @SPName, @ActionTypeID, @ActionNameEN, @ActionNameEN, @ActionNameEN, @ActionNameEN, @ActionNameEN, @ActionNameEN, @IsInternal, @ActionNameEN)