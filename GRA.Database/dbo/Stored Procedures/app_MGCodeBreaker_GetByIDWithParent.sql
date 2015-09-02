
CREATE PROCEDURE [dbo].[app_MGCodeBreaker_GetByIDWithParent] @MGID INT
AS
SELECT cb.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM MGCodeBreaker cb
INNER JOIN dbo.Minigame g ON cb.MGID = g.MGID
WHERE g.MGID = @MGID
