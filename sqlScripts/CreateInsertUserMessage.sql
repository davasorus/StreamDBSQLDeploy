CREATE PROCEDURE [dbo].[InsertUserMessage]
@User_Message nvarchar(MAX)
	as
	SET NOCOUNT ON;
	insert into dbo.UserMessage
	(User_Message)
	 Values (@User_Message)