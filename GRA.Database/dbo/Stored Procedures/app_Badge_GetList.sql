
CREATE PROCEDURE [dbo].[app_Badge_GetList] @ids VARCHAR(255) = ''
AS
SELECT *
FROM Badge
WHERE BID IN (
		(
			SELECT *
			FROM [dbo].[fnSplitBigInt](@ids)
			)
		)
