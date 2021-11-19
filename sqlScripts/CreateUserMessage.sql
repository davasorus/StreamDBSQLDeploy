CREATE TABLE [dbo].[UserMessage] (
    [ID]           INT            IDENTITY (1, 1) NOT NULL,
    [User_Message] NVARCHAR (MAX) NULL,
    [Date_Entered] DATETIME       DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_MessageID] PRIMARY KEY CLUSTERED ([ID] ASC)
);

