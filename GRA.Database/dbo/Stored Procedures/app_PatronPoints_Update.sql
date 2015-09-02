
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_Update]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronPoints_Update] (
	@PPID INT,
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
	@GameLevelActivityID INT
	)
AS
UPDATE PatronPoints
SET PID = @PID,
	NumPoints = @NumPoints,
	AwardDate = @AwardDate,
	AwardReason = @AwardReason,
	AwardReasonCd = @AwardReasonCd,
	BadgeAwardedFlag = @BadgeAwardedFlag,
	BadgeID = @BadgeID,
	PBID = @PBID,
	isReading = @isReading,
	LogID = @LogID,
	isEvent = @isEvent,
	EventID = @EventID,
	EventCode = @EventCode,
	isBookList = @isBookList,
	BookListID = @BookListID,
	isGame = @isGame,
	isGameLevelActivity = @isGameLevelActivity,
	GameID = @GameID,
	GameLevel = @GameLevel,
	GameLevelID = @GameLevelID,
	GameLevelActivityID = @GameLevelActivityID
WHERE PPID = @PPID
