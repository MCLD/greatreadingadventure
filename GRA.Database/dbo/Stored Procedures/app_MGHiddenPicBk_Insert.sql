
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPicBk_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPicBk_Insert] (
	@HPID INT,
	@MGID INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@HPBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGHiddenPicBk (
		HPID,
		MGID,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@HPID,
		@MGID,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @HPBID = SCOPE_IDENTITY()
END
