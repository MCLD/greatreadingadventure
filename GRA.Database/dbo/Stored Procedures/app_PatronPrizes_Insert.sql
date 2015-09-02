
/****** Object:  StoredProcedure [dbo].[app_PatronPrizes_Insert]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronPrizes_Insert] (
	@PID INT,
	@PrizeSource INT,
	@BadgeID INT,
	@DrawingID INT,
	@PrizeName VARCHAR(50),
	@RedeemedFlag BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@PPID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronPrizes (
		PID,
		PrizeSource,
		BadgeID,
		DrawingID,
		PrizeName,
		RedeemedFlag,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@PID,
		@PrizeSource,
		@BadgeID,
		@DrawingID,
		@PrizeName,
		@RedeemedFlag,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @PPID = SCOPE_IDENTITY()
END
