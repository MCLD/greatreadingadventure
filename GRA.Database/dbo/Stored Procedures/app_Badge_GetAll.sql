
CREATE PROCEDURE [dbo].[app_Badge_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Badge]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY AdminName
