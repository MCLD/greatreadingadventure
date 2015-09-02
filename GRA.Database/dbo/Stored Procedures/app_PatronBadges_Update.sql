
/****** Object:  StoredProcedure [dbo].[app_PatronBadges_Update]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronBadges_Update] (
	@PBID INT,
	@PID INT,
	@BadgeID INT,
	@DateEarned DATETIME
	)
AS
UPDATE PatronBadges
SET PID = @PID,
	BadgeID = @BadgeID,
	DateEarned = @DateEarned
WHERE PBID = @PBID
