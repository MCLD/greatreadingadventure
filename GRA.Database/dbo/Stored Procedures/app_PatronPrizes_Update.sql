
/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_Update]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronPrizes_Update] (
	@PPID INT,
	@PID INT,
	@PrizeSource INT,
	@BadgeID INT,
	@DrawingID INT,
	@PrizeName VARCHAR(50),
	@RedeemedFlag BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE PatronPrizes
SET PID = @PID,
	PrizeSource = @PrizeSource,
	BadgeID = @BadgeID,
	DrawingID = @DrawingID,
	PrizeName = @PrizeName,
	RedeemedFlag = @RedeemedFlag,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE PPID = @PPID
