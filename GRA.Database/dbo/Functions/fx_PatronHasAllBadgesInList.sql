
CREATE FUNCTION [dbo].[fx_PatronHasAllBadgesInList] (
	@PID INT,
	@BadgeList VARCHAR(500)
	)
RETURNS BIT
AS
BEGIN
	DECLARE @ret BIT

	SET @ret = 0

	IF (
			SELECT COUNT(DISTINCT BID)
			FROM Badge
			WHERE BID IN (
					SELECT *
					FROM fnSplitBigInt(@BadgeList)
					)
			) = (
			SELECT COUNT(DISTINCT BadgeID)
			FROM PatronBadges
			WHERE PID = @PID
				AND BadgeID IN (
					SELECT *
					FROM fnSplitBigInt(@BadgeList)
					)
			)
	BEGIN
		SET @ret = 1
	END

	RETURN @ret
END
