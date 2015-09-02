
/****** Object:  StoredProcedure [dbo].[app_Code_GetAllLibrarySystems]    Script Date: 01/05/2015 14:43:20 ******/
CREATE PROCEDURE [dbo].[app_Code_GetAllLibrarySystems] @TenID INT = NULL
AS
SELECT DISTINCT rtrim(ltrim(District)) AS LibSys
FROM Patron
WHERE rtrim(ltrim(District)) <> ''
	AND District IS NOT NULL
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY rtrim(ltrim(District))
