
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeGoals] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.AwardName
			ELSE @List + ', ' + p.AwardName
			END, '')
FROM Award p
WHERE p.TenID = @TenID
	AND p.BadgeID = @BID
	AND p.GoalPercent > 0
GROUP BY p.AwardName
