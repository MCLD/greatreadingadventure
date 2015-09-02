
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeEvents] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.EventTitle
			ELSE @List + ', ' + p.EventTitle
			END, '')
FROM Event p
WHERE p.TenID = @TenID
	AND p.BadgeID = @BID
ORDER BY p.EventTitle
