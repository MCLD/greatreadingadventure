
/****** Object:  StoredProcedure [dbo].[app_PrizeDrawingWinners_Update]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PrizeDrawingWinners_Update] (
	@PDWID INT,
	@PDID INT,
	@PatronID INT,
	@NotificationID INT,
	@PrizePickedUpFlag BIT,
	@LastModDate DATETIME,
	@LastModUser VARCHAR(50),
	@AddedDate DATETIME,
	@AddedUser VARCHAR(50)
	)
AS
UPDATE PrizeDrawingWinners
SET PDID = @PDID,
	PatronID = @PatronID,
	NotificationID = @NotificationID,
	PrizePickedUpFlag = @PrizePickedUpFlag,
	LastModDate = @LastModDate,
	LastModUser = @LastModUser,
	AddedDate = @AddedDate,
	AddedUser = @AddedUser
WHERE PDWID = @PDWID
