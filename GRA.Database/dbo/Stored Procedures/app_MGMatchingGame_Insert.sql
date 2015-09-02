
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_Insert]    Script Date: 01/05/2015 14:43:21 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGame_Insert] (
	@MGID INT,
	@CorrectRoundsToWinCount INT,
	@EasyGameSize INT,
	@MediumGameSize INT,
	@HardGameSize INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@MAGID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGMatchingGame (
		MGID,
		CorrectRoundsToWinCount,
		EasyGameSize,
		MediumGameSize,
		HardGameSize,
		EnableMediumDifficulty,
		EnableHardDifficulty,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@MGID,
		@CorrectRoundsToWinCount,
		@EasyGameSize,
		@MediumGameSize,
		@HardGameSize,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @MAGID = SCOPE_IDENTITY()
END
