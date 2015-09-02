
CREATE PROCEDURE [dbo].[app_Minigame_Insert] (
	@MiniGameType INT,
	@MiniGameTypeName VARCHAR(50),
	@AdminName VARCHAR(50),
	@GameName VARCHAR(50),
	@isActive BIT,
	@NumberPoints INT,
	@AwardedBadgeID INT,
	@Acknowledgements TEXT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
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
	@MGID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO Minigame (
		MiniGameType,
		MiniGameTypeName,
		AdminName,
		GameName,
		isActive,
		NumberPoints,
		AwardedBadgeID,
		Acknowledgements,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser,
		TenID,
		FldInt1,
		FldInt2,
		FldInt3,
		FldBit1,
		FldBit2,
		FldBit3,
		FldText1,
		FldText2,
		FldText3
		)
	VALUES (
		@MiniGameType,
		@MiniGameTypeName,
		@AdminName,
		@GameName,
		@isActive,
		@NumberPoints,
		@AwardedBadgeID,
		@Acknowledgements,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser,
		@TenID,
		@FldInt1,
		@FldInt2,
		@FldInt3,
		@FldBit1,
		@FldBit2,
		@FldBit3,
		@FldText1,
		@FldText2,
		@FldText3
		)

	SELECT @MGID = SCOPE_IDENTITY()
END
