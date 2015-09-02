
/****** Object:  StoredProcedure [dbo].[app_MGMixAndMatch_Update]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGMixAndMatch_Update] (
	@MMID INT,
	@MGID INT,
	@CorrectRoundsToWinCount INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGMixAndMatch
SET MGID = @MGID,
	CorrectRoundsToWinCount = @CorrectRoundsToWinCount,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE MMID = @MMID
