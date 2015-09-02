
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_Update]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGMatchingGame_Update] (
	@MAGID INT,
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
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGMatchingGame
SET MGID = @MGID,
	CorrectRoundsToWinCount = @CorrectRoundsToWinCount,
	EasyGameSize = @EasyGameSize,
	MediumGameSize = @MediumGameSize,
	HardGameSize = @HardGameSize,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE MAGID = @MAGID
