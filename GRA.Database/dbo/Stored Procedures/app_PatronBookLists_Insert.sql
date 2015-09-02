
/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_Insert]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronBookLists_Insert] (
	@PID INT,
	@BLBID INT,
	@BLID INT,
	@HasReadFlag BIT,
	@LastModDate DATETIME,
	@PBLBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronBookLists (
		PID,
		BLBID,
		BLID,
		HasReadFlag,
		LastModDate
		)
	VALUES (
		@PID,
		@BLBID,
		@BLID,
		@HasReadFlag,
		@LastModDate
		)

	SELECT @PBLBID = SCOPE_IDENTITY()
END
