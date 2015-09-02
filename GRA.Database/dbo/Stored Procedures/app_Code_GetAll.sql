
CREATE PROCEDURE [dbo].[app_Code_GetAll] @TenID INT = NULL
AS
SELECT *
FROM [Code]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
