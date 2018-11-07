/*<info serverAlias="DB053-bodb_ChatBot" executers="bodbDownlineNet" viewers="vnteam" selecters=""></info>*/

USE [bodb_ChatBot]
GO

ALTER PROCEDURE [dbo].[Chatbot_InsertUserAction]
	@UserId INT,
	@ActionId INT,
	@UserResponse NVARCHAR(255),
	@CompiledCommand NVARCHAR(255),
	@Status	TINYINT,
	@ChatTime DATETIME
AS
/*
	Creator:	20181105@Kan
	Task:		Support chatbot conversation
	Database:	DB053.bodb_ChatBot
	
	Revisions:
		- 20181105@Kan: Create new [RedmineId: 103467]
		
	Parameter's Explanation:
		- 
		
	Developer's Notes:
		- Notes
*/
BEGIN
	SET NOCOUNT ON;

	-- IF(@Status = 0 AND EXISTS(SELECT 1
	-- 	FROM dbo.UserAction WITH (NOLOCK)
	-- 	WHERE UserId = @UserId AND ActionId = @ActionId AND [Status] = @Status))
	-- BEGIN
	-- 	UPDATE dbo.UserAction WITH (ROWLOCK, UPDLOCK)
	-- 	SET [Status] = 2
	-- 	WHERE UserId = @UserId AND ActionId = @ActionId AND [Status] = @Status;
	-- END

	INSERT INTO dbo.UserAction
		(UserId, ActionId, UserResponse, CompiledCommand, [Status], ChatTime)
	VALUES
		(@UserId, @ActionId, @UserResponse, @CompiledCommand, @Status, @ChatTime);
END
GO