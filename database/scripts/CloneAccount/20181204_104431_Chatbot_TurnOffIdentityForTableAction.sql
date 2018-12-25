/*
	CreatedDate: 20181204@Kan
	Task: Edit Action table, remove Identity of ActionId column
	DB: DBBOT/bodb_ChatBot
*/

USE [bodb_ChatBot]
GO

-- Create new Action table
CREATE TABLE [dbo].[Action_New](
	[ActionId] [INT] NOT NULL,
	[ActionName] [VARCHAR](255) NOT NULL,
	[Type] [TINYINT] NOT NULL,
	[Command] [VARCHAR](50) NULL,
	[DisplayOrder] [INT] NULL,
	[FunctionId] [INT] NOT NULL,
	[ParentId] [INT] NOT NULL,
	[IsEndAction] [BIT] NULL,
	[Status] [TINYINT] NOT NULL
 CONSTRAINT [PK_Action_New] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [DATA]
) ON [DATA]

-- Import data from Action to Action_New
INSERT INTO [dbo].[Action_New]
	(ActionId, ActionName, [Type], Command, DisplayOrder, FunctionId, ParentId, IsEndAction, [Status])
SELECT ActionId, ActionName, [Type], Command, DisplayOrder, FunctionId, ParentId, IsEndAction, [Status]
FROM [dbo].[Action] WITH (NOLOCK)

-- Change name of Action table to Action_Old
EXEC sp_rename 'Action', 'Action_Old'

-- Change name of Action_New table to Action
EXEC sp_rename 'Action_New', 'Action'
GO
