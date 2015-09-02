
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatchItems_Insert]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatchItems_Insert] (
	@MMID INT,
	@MGID INT,
	@ItemImage VARCHAR(150),
	@EasyLabel VARCHAR(150),
	@MediumLabel VARCHAR(150),
	@HardLabel VARCHAR(150),
	@AudioEasy VARCHAR(150),
	@AudioMedium VARCHAR(150),
	@AudioHard VARCHAR(150),
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@MMIID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGMixAndMatchItems (
		MMID,
		MGID,
		ItemImage,
		EasyLabel,
		MediumLabel,
		HardLabel,
		AudioEasy,
		AudioMedium,
		AudioHard,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MMID,
		@MGID,
		@ItemImage,
		@EasyLabel,
		@MediumLabel,
		@HardLabel,
		@AudioEasy,
		@AudioMedium,
		@AudioHard,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @MMIID = SCOPE_IDENTITY()
END
