
CREATE PROCEDURE [dbo].[app_Badge_GetVisibleCount] @TenID INT = NULL
AS
SELECT COUNT(BID)
FROM [Badge]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
	AND [HiddenFromPublic] != 1