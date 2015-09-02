
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_Insert]    Script Date: 01/05/2015 14:43:25 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_Insert] (
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
	@AwardBadgeIDBonus INT,
	@PGLID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO ProgramGameLevel (
		PGID,
		LevelNumber,
		LocationX,
		LocationY,
		PointNumber,
		Minigame1ID,
		Minigame2ID,
		AwardBadgeID,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		LocationXBonus,
		LocationYBonus,
		Minigame1IDBonus,
		Minigame2IDBonus,
		AwardBadgeIDBonus
		)
	VALUES (
		@PGID,
		(
			SELECT isnull(Max(LevelNumber), 0) + 1
			FROM ProgramGameLevel
			WHERE PGID = @PGID
			),
		@LocationX,
		@LocationY,
		@PointNumber,
		@Minigame1ID,
		@Minigame2ID,
		@AwardBadgeID,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@LocationXBonus,
		@LocationYBonus,
		@Minigame1IDBonus,
		@Minigame2IDBonus,
		@AwardBadgeIDBonus
		)

	SELECT @PGLID = SCOPE_IDENTITY()
END
