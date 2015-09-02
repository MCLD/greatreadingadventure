
CREATE PROCEDURE [dbo].[app_Avatar_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Avatar]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
