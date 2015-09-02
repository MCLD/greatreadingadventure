
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeReading] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN g.GameName
			ELSE @List + ', ' + g.GameName
			END, '')
FROM dbo.ProgramGameLevel p
INNER JOIN ProgramGame g ON g.PGID = p.PGID
WHERE g.TenID = @TenID
	AND p.AwardBadgeID = @BID
	OR AwardBadgeIDBonus = @BID

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.AwardName
			ELSE @List + ', ' + p.AwardName
			END, '')
FROM Award p
WHERE p.TenID = @TenID
	AND p.BadgeID = @BID
	AND p.NumPoints > 0
GROUP BY p.AwardName
