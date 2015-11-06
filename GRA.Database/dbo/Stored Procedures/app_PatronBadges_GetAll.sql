
CREATE PROCEDURE [dbo].[app_PatronBadges_GetAll] @PID INT = 0
AS
SELECT ROW_NUMBER() OVER (
		ORDER BY DateEarned,
			PBID
		) AS Rank,
	pb.*,
	b.UserName AS Title
FROM [PatronBadges] pb
LEFT JOIN Badge b ON pb.BadgeID = b.BID
WHERE PID = @PID
ORDER BY Rank DESC