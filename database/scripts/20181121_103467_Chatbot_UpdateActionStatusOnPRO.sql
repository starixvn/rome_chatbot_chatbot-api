/*
	CreatedDate: 20181121@Kan
	Task: Update Status of all Actions from 1 to 0
	DB: DBBOT/bodb_ChatBot
    Column explanation:
    - Type: 1: button, 2: message, 3 uri
    - Status: 0: PRO, 1: UAT, 2: Hide
*/

USE [bodb_ChatBot]
GO

UPDATE dbo.Action WITH (ROWLOCK, UPDLOCK)
SET [Status] = 0
