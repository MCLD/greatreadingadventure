
/****** Object:  StoredProcedure [dbo].[app_PatronBadges_Insert]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronBadges_Insert] (
	@PID INT,
	@BadgeID INT,
	@DateEarned DATETIME,
	@PBID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronBadges (
		PID,
		BadgeID,
		DateEarned
		)
	VALUES (
		@PID,
		@BadgeID,
		@DateEarned
		)

	SELECT @PBID = SCOPE_IDENTITY()
END
