
CREATE PROCEDURE [dbo].[app_MGMixAndMatch_GetByIDWithParent] @MGID INT
AS
SELECT mm.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM MGMixAndMatch mm
INNER JOIN dbo.Minigame g ON mm.MGID = g.MGID
WHERE g.MGID = @MGID
