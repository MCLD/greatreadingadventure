
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeBookLists] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = LTRIM(RTRIM(STUFF((
					SELECT ', ' + p.ListName
					FROM BookList p
					WHERE p.TenID = @TenID
						AND p.AwardBadgeID = @BID
					GROUP BY p.ListName
					ORDER BY p.ListName
					FOR XML PATH('')
					), 1, 1, '')))
