/*
	Created by: Alvin
	Date: 20181128
	Task: Update LogAction's Translation for Chatbot Self-destruction period
	DB: DBLOG / bodb_CustomerLog
*/
USE [bodb_CustomerLog]
GO

DECLARE @ActionNameCH NVARCHAR(256)
	,@ActionNameCS NVARCHAR(256)
	,@ActionNameTH NVARCHAR(256)
	,@ActionNameVI NVARCHAR(256)

SELECT @ActionNameCH = N'更新Chatbot 自動銷毀期限'
	,@ActionNameCS = N'更新Chatbot 自动销毁期限'
	,@ActionNameTH = N'อัปเดทระยะเวลาการทำลายตนเองของ Chatbot''s'
	,@ActionNameVI = N'Cập nhật thời gian tự xóa của Chatbot'

UPDATE dbo.LogAction
SET ActionNameCH = @ActionNameCH
	,ActionNameCS = @ActionNameCS
	,ActionNameTH = @ActionNameTH
	,ActionNameVI = @ActionNameVI
WHERE ActionID = 684
