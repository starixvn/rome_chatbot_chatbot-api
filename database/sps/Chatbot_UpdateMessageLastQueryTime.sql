/*<info serverAlias="DBAGE-bodb_ChatBot" executers="bodbDownlineNet" selecters=""></info>*/

USE [bodb_ChatBot]
GO

/****** Object:  StoredProcedure [dbo].[Chatbot_UpdateMessageLastQueryTime]     Script Date: 8/16/2018 3:46:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*
	Creator:	20180727@Harrison.Nguyen
	Task:		Description - [Redmine: #99375]
	Database:	DB053.bodb_ChatBot
	
	Revisions:
		- 20180816@Harrison.Nguyen:	Initial
		
	Parameter's Explaination:
		- @Example:			NULL		- Explaination [DEFAULT]
		
	Developer's Notes:
		- Notes
*/

ALTER PROCEDURE [dbo].[Chatbot_UpdateMessageLastQueryTime]
				@UserId			INT
				,@FunctionId	INT
				,@MessageType		INT
				,@LastQueryTime DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN 
		IF NOT EXISTS (
			SELECT TOP(1) [LastQueryTime]
			FROM LastQueryUserMessage AS A WITH (NOLOCK)
			WHERE A.UserId = @UserId AND (A.FunctionId = @FunctionId OR A.MessageType = @MessageType))
		BEGIN
			INSERT INTO LastQueryUserMessage
			VALUES (@UserId, @FunctionId, @MessageType, @LastQueryTime)
		END
		ELSE
			UPDATE LastQueryUserMessage WITH (ROWLOCK, UPDLOCK)
			SET [LastQueryTime] = @LastQueryTime
			WHERE 
				[UserId] = @UserId 
				AND ([FunctionId] = @FunctionId
				OR [MessageType] = @MessageType)
		END
 			
END

GO


