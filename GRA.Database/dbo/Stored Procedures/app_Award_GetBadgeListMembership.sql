
CREATE PROCEDURE [dbo].[app_Award_GetBadgeListMembership] @BadgeList VARCHAR(500) = '',
	@TenID INT
AS
SELECT BID,
	AdminName,
	CASE 
		WHEN CHARINDEX(CONVERT(VARCHAR(10), BID) + ',', ',' + @BadgeList + ',') > 0
			THEN 1
		ELSE 0
		END AS isMember
FROM Badge
WHERE TenID = @TenID
ORDER BY AdminName,
	BID
