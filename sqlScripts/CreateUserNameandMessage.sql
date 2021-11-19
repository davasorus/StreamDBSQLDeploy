CREATE TABLE [dbo].[UserNameandMessage] (
    [ID]           INT            IDENTITY (1, 1) NOT NULL,
    [User_Name]    NVARCHAR (50)  NULL,
    [User_Message] NVARCHAR (MAX) NULL,
    [User_ID]      INT            NOT NULL,
    [Message_ID]   INT            NOT NULL,
    CONSTRAINT [FK_UserNameandMessage_ToMessageTable] FOREIGN KEY ([Message_ID]) REFERENCES [dbo].[UserMessage] ([ID]),
    CONSTRAINT [FK_UserNameandMessage_ToUserTable] FOREIGN KEY ([User_ID]) REFERENCES [dbo].[UserInformation] ([ID])
);

