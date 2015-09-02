
CREATE PROCEDURE [dbo].[app_MGHiddenPic_GetByIDWithParent] @MGID INT
AS
SELECT mg.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM MGHiddenPic mg
INNER JOIN dbo.Minigame g ON mg.MGID = g.MGID
WHERE g.MGID = @MGID
