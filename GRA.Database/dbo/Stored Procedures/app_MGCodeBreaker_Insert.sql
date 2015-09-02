
/****** Object:  StoredProcedure [dbo].[app_MGCodeBreaker_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGCodeBreaker_Insert] (
	@MGID INT,
	@EasyString VARCHAR(250),
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@MediumString VARCHAR(250),
	@HardString VARCHAR(250),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@CBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGCodeBreaker (
		MGID,
		EasyString,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		MediumString,
		HardString,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@EasyString,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@MediumString,
		@HardString,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @CBID = SCOPE_IDENTITY()
END
