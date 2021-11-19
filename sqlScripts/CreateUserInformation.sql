CREATE TABLE [dbo].[UserInformation] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [UserName]         NVARCHAR (MAX) NULL,
    [Is_Subscribed]    NVARCHAR (50)  NULL,
    [Sub_Length]       INT            NULL,
    [Is_Moderator]     NVARCHAR (50)  NULL,
    [Is_VIP]           NVARCHAR (50)  NULL,
    [Given_Bits]       NVARCHAR (50)  NULL,
    [Bit_Amount]       INT            NULL,
    [Is_Founder]       NVARCHAR (50)  NULL,
    [DateTime_Created] DATETIME       DEFAULT (getdate()) NULL,
    [DateTime_Updated] DATETIME       NULL,
    CONSTRAINT [PK_UserID] PRIMARY KEY CLUSTERED ([ID] ASC)
);

