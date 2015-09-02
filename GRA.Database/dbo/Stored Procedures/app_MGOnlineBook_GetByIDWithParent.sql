
CREATE PROCEDURE [dbo].[app_MGOnlineBook_GetByIDWithParent] @MGID INT
AS
SELECT ob.*,
	MiniGameTypeName,
	AdminName,
	GameName,
	isActive,
	NumberPoints,
	AwardedBadgeID,
	Acknowledgements
FROM [MGOnlineBook] ob
INNER JOIN dbo.Minigame g ON ob.MGID = g.MGID
WHERE g.MGID = @MGID
