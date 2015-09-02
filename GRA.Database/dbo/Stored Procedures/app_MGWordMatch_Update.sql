
/****** Object:  StoredProcedure [dbo].[app_MGWordMatch_Update]    Script Date: 01/05/2015 14:43:22 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_MGWordMatch_Update] (
	@WMID INT,
	@MGID INT,
	@CorrectRoundsToWinCount INT,
	@NumOptionsToChooseFrom INT,
	@EnableMediumDifficulty BIT,
	@EnableHardDifficulty BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE MGWordMatch
SET MGID = @MGID,
	CorrectRoundsToWinCount = @CorrectRoundsToWinCount,
	NumOptionsToChooseFrom = @NumOptionsToChooseFrom,
	EnableMediumDifficulty = @EnableMediumDifficulty,
	EnableHardDifficulty = @EnableHardDifficulty,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE WMID = @WMID
