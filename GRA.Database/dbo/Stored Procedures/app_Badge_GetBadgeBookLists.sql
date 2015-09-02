
------------------------------------------------------------------
CREATE PROCEDURE [dbo].[app_Badge_GetBadgeBookLists] @TenID INT,
	@BID INT = 0,
	@List VARCHAR(2000) OUTPUT
AS
SET NOCOUNT ON
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT @List = ''

SELECT @List = COALESCE(CASE 
			WHEN @List = ''
				THEN p.ListName
			ELSE @List + ', ' + p.ListName
			END, '')
FROM BookList p
WHERE p.TenID = @TenID
	AND p.AwardBadgeID = @BID
ORDER BY p.ListName
