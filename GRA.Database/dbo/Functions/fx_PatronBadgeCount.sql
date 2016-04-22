
/* =============================================
-- Return a count of how many badges Patron ID 
-- @PID has out of the comma-separated text 
-- list in @BadgeList
-- ============================================= */
CREATE FUNCTION [dbo].[fx_PatronBadgeCount] (
	@PID INT,
	@BadgeList NVARCHAR(4000)
	)
RETURNS INT
AS
BEGIN
	DECLARE @return INT

	SELECT @return = COUNT(DISTINCT BadgeID)
	FROM PatronBadges
	WHERE PID = @PID
		AND BadgeID IN (
			SELECT *
			FROM fnSplitBigInt(@BadgeList)
			)

	RETURN @return
END