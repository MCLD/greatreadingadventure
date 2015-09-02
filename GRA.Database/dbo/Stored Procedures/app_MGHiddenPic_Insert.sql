
/****** Object:  StoredProcedure [dbo].[app_MGHiddenPic_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGHiddenPic_Insert] (
	@MGID INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@EasyDictionary TEXT,
	@MediumDictionary TEXT,
	@HardDictionary TEXT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@HPID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGHiddenPic (
		MGID,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		EasyDictionary,
		MediumDictionary,
		HardDictionary,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@EasyDictionary,
		@MediumDictionary,
		@HardDictionary,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @HPID = SCOPE_IDENTITY()
END
