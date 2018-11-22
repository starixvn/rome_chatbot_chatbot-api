/*
	CreatedDate: 20181105@Kan
	Task: Add tables Action, UserAction to support Chatbot conversation
	DB: DBBOT/bodb_ChatBot
*/

USE [bodb_ChatBot]
GO

CREATE TABLE [dbo].[Action](
	[ActionId] [int] IDENTITY(1,1) NOT NULL,
	[ActionName] [varchar](255) NOT NULL,
	[Type] [tinyint] NOT NULL,
	[Command] [varchar](50) NULL,
	[DisplayOrder] [int] NULL,
	[FunctionId] [int] NOT NULL,
	[ParentId] [int] NOT NULL,
	[IsEndAction] [bit] NULL,
	[Status] [tinyint] NOT NULL,
 CONSTRAINT [PK_Action] PRIMARY KEY CLUSTERED 
(
	[ActionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [DATA]
) ON [DATA]
GO

CREATE TABLE [dbo].[UserAction](
	[UserActionId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ActionId] [int] NOT NULL,
	[UserResponse] [nvarchar](255) NULL,
	[CompiledCommand] [nvarchar](255) NULL,
	[ChatTime] [datetime] NULL,
	[Status] [tinyint],
 CONSTRAINT [PK_UserAction] PRIMARY KEY NONCLUSTERED 
(
	[UserActionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [DATA]
) ON [DATA]
GO
