
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_Update]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_Update] (
	@PGLID INT,
	@PGID INT,
	@LevelNumber INT,
	@LocationX INT,
	@LocationY INT,
	@PointNumber INT,
	@Minigame1ID INT,
	@Minigame2ID INT,
	@AwardBadgeID INT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@LocationXBonus INT,
	@LocationYBonus INT,
	@Minigame1IDBonus INT,
	@Minigame2IDBonus INT,
	@AwardBadgeIDBonus INT
	)
AS
UPDATE ProgramGameLevel
SET PGID = @PGID,
	LevelNumber = @LevelNumber,
	LocationX = @LocationX,
	LocationY = @LocationY,
	PointNumber = @PointNumber,
	Minigame1ID = @Minigame1ID,
	Minigame2ID = @Minigame2ID,
	AwardBadgeID = @AwardBadgeID,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser,
	LocationXBonus = @LocationXBonus,
	LocationYBonus = @LocationYBonus,
	Minigame1IDBonus = @Minigame1IDBonus,
	Minigame2IDBonus = @Minigame2IDBonus,
	AwardBadgeIDBonus = @AwardBadgeIDBonus
WHERE PGLID = @PGLID
