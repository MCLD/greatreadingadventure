
CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_GetFilteredDistrictDDValues] @City VARCHAR(50) = '',
	@TenID INT = NULL
AS
SELECT DISTINCT DistrictID AS CID,
	c.Code AS Code, c.[Description] as [Description]
FROM LibraryCrosswalk w
INNER JOIN Code c ON w.DistrictID = c.CID
WHERE (
		City = @City
		OR @City IS NULL
		OR @City = ''
		)
	AND (
		w.TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY [Description]