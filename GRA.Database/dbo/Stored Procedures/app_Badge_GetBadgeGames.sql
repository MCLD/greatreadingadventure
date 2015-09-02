
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeGames] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.GameName
			ELSE @List + ', ' + p.GameName
			END, '')
FROM Minigame p
WHERE p.TenID = @TenID
	AND p.AwardedBadgeID = @BID
ORDER BY p.GameName
