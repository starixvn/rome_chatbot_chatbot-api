/*<info serverAlias="DBAGE-bodb_ChatBot" executers="bodbDownlineNet" viewers="vnteam" selecters=""></info>*/

USE [bodb_ChatBot]
GO

ALTER PROCEDURE [dbo].[Chatbot_GetUserActionsInChain]
	@UserId INT,
	@TimeOutInMinutes INT = 3
AS
/*
	Creator:	20181106@Kan
	Task:		Support chatbot conversation
	Database:	DBAGE.bodb_ChatBot
	
	Revisions:
		- 20181106@Kan: Create new [RedmineId: 103467]
		
	Parameter's Explanation:
		- 
		
	Developer's Notes:
		- Notes
*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @TimeOutTime SMALLDATETIME = DATEADD(MINUTE, @TimeOutInMinutes * -1, GETDATE());
	DECLARE @ActionId INT = 0;

	SELECT TOP 1 @ActionId = ActionId
	FROM UserAction WITH (NOLOCK)
	WHERE UserId = @UserId AND ChatTime >= @TimeOutTime
	ORDER BY UserActionId DESC;

	IF (@ActionId = 0)
	BEGIN
		RETURN;
	END

	DECLARE @TempActionId INT = @ActionId;
	DECLARE @TempParentId INT = 0;
	DECLARE @TempAction TABLE
	(
		ActionId INT
	);
	INSERT INTO @TempAction(ActionId) VALUES(@TempActionId);

	WHILE (1 = 1)
	BEGIN
		SELECT @TempParentId = ParentId
		FROM dbo.[Action] WITH (NOLOCK)
		WHERE ActionId = @TempActionId;

		IF (@TempParentId = 0)
		BEGIN
			BREAK;
		END

		SET @TempActionId = @TempParentId;
		INSERT INTO @TempAction(ActionId) VALUES(@TempActionId);
	END

	-- IF (@TempActionId = @ActionId)
	-- BEGIN
	-- 	RETURN;
	-- END

	DECLARE @TempUserActionId INT = 0;

	SELECT TOP 1
		@TempUserActionId = u.UserActionId
	FROM UserAction u WITH (NOLOCK)
	WHERE u.UserId = @UserId AND u.ActionId = @TempActionId
	ORDER BY UserActionId DESC;

	IF (@TempUserActionId > 0)
	BEGIN
		SELECT u.UserActionId, u.UserId, u.ActionId, u.UserResponse, u.CompiledCommand, u.ChatTime
		FROM UserAction u WITH (NOLOCK) INNER JOIN @TempAction t
			ON u.ActionId = t.ActionId
		WHERE u.UserActionId >= @TempUserActionId;
	END
END
GO