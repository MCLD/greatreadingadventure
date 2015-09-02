
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_ProgramGame_Insert] (
	@GameName VARCHAR(50),
	@MapImage VARCHAR(50),
	@BonusMapImage VARCHAR(50),
	@BoardWidth INT,
	@BoardHeight INT,
	@BonusLevelPointMultiplier MONEY,
	@LevelCompleteImage VARCHAR(50),
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@LastModDate DATETIME,
	@TenID INT = 0,
	@FldInt1 INT = 0,
	@FldInt2 INT = 0,
	@FldInt3 INT = 0,
	@FldBit1 BIT = 0,
	@FldBit2 BIT = 0,
	@FldBit3 BIT = 0,
	@FldText1 TEXT = '',
	@FldText2 TEXT = '',
	@FldText3 TEXT = '',
	@Minigame1ID INT = 0,
	@Minigame2ID INT = 0,
	@PGID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO ProgramGame (
		GameName,
		MapImage,
		BonusMapImage,
		BoardWidth,
		BoardHeight,
		BonusLevelPointMultiplier,
		LevelCompleteImage,
		LastModUser,
		AddedDate,
		AddedUser,
		LastModDate,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3,
		Minigame1ID,
		Minigame2ID
		)
	VALUES (
		@GameName,
		@MapImage,
		@BonusMapImage,
		@BoardWidth,
		@BoardHeight,
		@BonusLevelPointMultiplier,
		@LevelCompleteImage,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@LastModDate,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3,
		@Minigame1ID,
		@Minigame2ID
		)

	SELECT @PGID = SCOPE_IDENTITY()
END
