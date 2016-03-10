
CREATE PROCEDURE [dbo].[app_AvatarPart_GetQualifiedByPatron] @PID INT = NULL
AS
SELECT
	a.*
FROM [PatronBadges] pb
INNER JOIN AvatarPart a ON pb.BadgeID = a.BadgeID
WHERE pb.PID = @PID
UNION ALL
SELECT 
    a.*
FROM [AvatarPart] a
WHERE a.BadgeID = -1
ORDER BY ComponentID DESC