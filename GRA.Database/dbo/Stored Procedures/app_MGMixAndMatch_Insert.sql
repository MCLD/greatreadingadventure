
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_Insert]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatch_Insert] (
	@MGID INT,
	@CorrectRoundsToWinCount INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@MMID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGMixAndMatch (
		MGID,
		CorrectRoundsToWinCount,
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
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @MMID = SCOPE_IDENTITY()
END
