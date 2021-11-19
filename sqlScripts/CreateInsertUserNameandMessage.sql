CREATE PROCEDURE [dbo].[InsertUserNameandMessage]
@User_Name nvarchar(50),
@User_Message nvarchar(Max),
@User_ID int,
@Message_ID int
	as
	SET NOCOUNT ON;


	SET @User_ID = (
	select DISTINCT TOP 1 ID
	from UserInformation
	where @User_Name = UserInformation.UserName
	)

	SET @Message_ID = (
	select DISTINCT TOP 1 ID
	from UserMessage 
	where @User_Message = UserMessage.User_Message
	)

	

	insert into dbo.UserNameandMessage (User_Name,User_Message,User_ID,Message_ID)
	 Values
	 (@User_Name,@User_Message,@User_ID,@Message_ID)