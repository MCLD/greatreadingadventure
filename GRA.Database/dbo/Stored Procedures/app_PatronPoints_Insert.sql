
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_Insert]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronPoints_Insert] (
	@PID INT,
	@NumPoints INT,
	@AwardDate DATETIME,
	@AwardReason VARCHAR(50),
	@AwardReasonCd INT,
	@BadgeAwardedFlag BIT,
	@BadgeID INT,
	@PBID INT,
	@isReading BIT,
	@LogID INT,
	@isEvent BIT,
	@EventID INT,
	@EventCode VARCHAR(50),
	@isBookList BIT,
	@BookListID INT,
	@isGame BIT,
	@isGameLevelActivity BIT,
	@GameID INT,
	@GameLevel INT,
	@GameLevelID INT,
	@GameLevelActivityID INT,
	@PPID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronPoints (
		PID,
		NumPoints,
		AwardDate,
		AwardReason,
		AwardReasonCd,
		BadgeAwardedFlag,
		BadgeID,
		PBID,
		isReading,
		LogID,
		isEvent,
		EventID,
		EventCode,
		isBookList,
		BookListID,
		isGame,
		isGameLevelActivity,
		GameID,
		GameLevel,
		GameLevelID,
		GameLevelActivityID
		)
	VALUES (
		@PID,
		@NumPoints,
		@AwardDate,
		@AwardReason,
		@AwardReasonCd,
		@BadgeAwardedFlag,
		@BadgeID,
		@PBID,
		@isReading,
		@LogID,
		@isEvent,
		@EventID,
		@EventCode,
		@isBookList,
		@BookListID,
		@isGame,
		@isGameLevelActivity,
		@GameID,
		@GameLevel,
		@GameLevelID,
		@GameLevelActivityID
		)

	SELECT @PPID = SCOPE_IDENTITY()
END
