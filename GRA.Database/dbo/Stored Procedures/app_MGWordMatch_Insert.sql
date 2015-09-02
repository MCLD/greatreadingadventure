
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_Insert]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_MGWordMatch_Insert] (
	@MGID INT,
	@CorrectRoundsToWinCount INT,
	@NumOptionsToChooseFrom INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@WMID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO MGWordMatch (
		MGID,
		CorrectRoundsToWinCount,
		NumOptionsToChooseFrom,
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
		@NumOptionsToChooseFrom,
		@EnableMediumDifficulty,
		@EnableHardDifficulty,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @WMID = SCOPE_IDENTITY()
END
