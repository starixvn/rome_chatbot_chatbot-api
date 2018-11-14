/*
	CreatedDate: 20181105@Kan
	Task: Insert default data into Action table
	DB: DBBOT/bodb_ChatBot
    Column explanation:
    - Type: 1: button, 2: message, 3 uri
    - Status: 0: PRO, 1: UAT, 2: Hide
*/

USE [bodb_ChatBot]
GO

DECLARE @OutstandingFunctionId INT,
        @TodayWinLossFunctionId INT,
        @BalanceInfoFunctionId INT,
        @ViewOutstandingReportActionId INT,
        @SpecificDirectDownlineActionId INT;

-- Get FunctionId
SELECT @OutstandingFunctionId = FunctionId
FROM dbo.[Function] WITH (NOLOCK)
WHERE FunctionName = 'Outstanding';

SELECT @TodayWinLossFunctionId = FunctionId
FROM dbo.[Function] WITH (NOLOCK)
WHERE FunctionName = 'Today Win Loss';

SELECT @BalanceInfoFunctionId = FunctionId
FROM dbo.[Function] WITH (NOLOCK)
WHERE FunctionName = 'Balance Info';

-- Insert root Actions
INSERT INTO dbo.Action(ActionName, [Type], Command, DisplayOrder, FunctionId, ParentId, IsEndAction, [Status])
VALUES ('View Outstanding Report', 1, '/os', 10, @OutstandingFunctionId, 0, 0, 1);
SELECT @ViewOutstandingReportActionId = SCOPE_IDENTITY();

INSERT INTO dbo.Action(ActionName, [Type], Command, DisplayOrder, FunctionId, ParentId, IsEndAction, [Status])
VALUES ('View Today Win Loss', 1, '/wl', 20, @TodayWinLossFunctionId, 0, 1, 1);

INSERT INTO dbo.Action(ActionName, [Type], Command, DisplayOrder, FunctionId, ParentId, IsEndAction, [Status])
VALUES ('View Balance Info', 1, '/bal', 30, @BalanceInfoFunctionId, 0, 1, 1);

-- Insert Outstanding actions
INSERT INTO dbo.Action(ActionName, [Type], Command, DisplayOrder, FunctionId, ParentId, IsEndAction, [Status])
VALUES ('Specific Direct Downline', 1, '', 60, @OutstandingFunctionId, @ViewOutstandingReportActionId, 0, 1);
SELECT @SpecificDirectDownlineActionId = SCOPE_IDENTITY();

INSERT INTO dbo.Action(ActionName, [Type], Command, DisplayOrder, FunctionId, ParentId, IsEndAction, [Status])
VALUES ('Top 50 Outstanding', 1, '50', 70, @OutstandingFunctionId, @ViewOutstandingReportActionId, 1, 1);

INSERT INTO dbo.Action(ActionName, [Type], Command, DisplayOrder, FunctionId, ParentId, IsEndAction, [Status])
VALUES ('Please input username', 2, '', '', @OutstandingFunctionId, @SpecificDirectDownlineActionId, 1, 1);
