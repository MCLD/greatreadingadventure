
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeEvents] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = LTRIM(RTRIM(STUFF((
					SELECT ', ' + e.EventTitle
					FROM Event e
					WHERE e.TenID = @TenID
						AND e.BadgeID = @BID
						AND e.HiddenFromPublic != 1
					GROUP BY e.EventTitle
					ORDER BY e.EventTitle
					FOR XML PATH('')
					), 1, 1, '')))
