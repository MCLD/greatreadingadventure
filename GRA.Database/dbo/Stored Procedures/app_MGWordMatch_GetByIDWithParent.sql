
CREATE PROCEDURE [dbo].[app_MGWordMatch_GetByIDWithParent] @MGID INT
AS
SELECT mm.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM MGWordMatch mm
INNER JOIN dbo.Minigame g ON mm.MGID = g.MGID
WHERE g.MGID = @MGID
