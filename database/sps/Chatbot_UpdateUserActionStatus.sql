/*<info serverAlias="DBAGE-bodb_ChatBot" executers="bodbDownlineNet" viewers="vnteam" selecters=""></info>*/

USE [bodb_ChatBot]
GO

ALTER PROCEDURE [dbo].[Chatbot_UpdateUserActionStatus]
	@UserActionId INT,
	@Status TINYINT = 0
AS
/*
	Creator:	20181107@Kan
	Task:		Support chatbot conversation
	Database:	DBAGE.bodb_ChatBot
	
	Revisions:
		- 20181107@Kan: Create new [RedmineId: 103467]
		
	Parameter's Explanation:
		- 
		
	Developer's Notes:
		- Notes
*/
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.UserAction WITH (ROWLOCK, UPDLOCK)
    SET [Status] = @Status
    WHERE UserActionId = @UserActionId
END
GO