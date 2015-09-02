
/****** Object:  StoredProcedure [dbo].[app_MGChooseAdv_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGChooseAdv_Insert] (
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@CAID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGChooseAdv (
		MGID,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @CAID = SCOPE_IDENTITY()
END
