
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_Insert]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawingWinners_Insert] (
	@PDID INT,
	@PatronID INT,
	@NotificationID INT,
	@PrizePickedUpFlag BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50),
	@PDWID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PrizeDrawingWinners (
		PDID,
		PatronID,
		NotificationID,
		PrizePickedUpFlag,
		LastModDate,
		LastModUser,
		AddedDate,
		AddedUser
		)
	VALUES (
		@PDID,
		@PatronID,
		@NotificationID,
		@PrizePickedUpFlag,
		@LastModDate,
		@LastModUser,
		@AddedDate,
		@AddedUser
		)

	SELECT @PDWID = SCOPE_IDENTITY()
END
