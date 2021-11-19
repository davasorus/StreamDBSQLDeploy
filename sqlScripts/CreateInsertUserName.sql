CREATE PROCEDURE [dbo].[InsertUserName]
@userName nvarchar(max),
@Is_Subscribed nvarchar(50),
@Sub_Length int,
@Is_Moderator nvarchar(50),
@Is_VIP nvarchar(50),
@Given_Bits nvarchar(50),
@Bit_Amount int,
@Founder Nvarchar(50)
	as
	SET NOCOUNT ON;

BEGIN
	
	DECLARE @UpdateDate DateTime;
	


	if 
	exists(select * from UserInformation where @userName = UserName)
	BEGIN
	
	SET @UpdateDate = GETDATE();

		update UserInformation
		SET Is_Subscribed = @Is_Subscribed, 
		Sub_Length = @Sub_Length,
		Is_Moderator = @Is_Moderator,
	    Is_VIP = @Is_VIP,
	    Given_Bits = @Given_Bits,
	    Bit_Amount = @Bit_Amount,
	    Is_Founder = @Founder,
		DateTime_Updated = @UpdateDate

		where @userName = UserInformation.UserName
	end

		else
		BEGIN

		insert into dbo.UserInformation
		(UserName,
		Is_Subscribed,
		Sub_Length,
		Is_Moderator,
		Is_VIP,
		Given_Bits,
		Bit_Amount,
		Is_Founder)
		Values (@UserName,@Is_Subscribed,@Sub_Length,@Is_Moderator,@Is_VIP,@Given_Bits,@Bit_Amount,@Founder)

		END
END