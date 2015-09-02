
/****** Object:  StoredProcedure [dbo].[app_Code_GetAllSchools]    Script Date: 01/05/2015 14:43:20 ******/
CREATE PROCEDURE [dbo].[app_Code_GetAllSchools] @TenID INT = NULL
AS
SELECT DISTINCT rtrim(ltrim(SchoolName)) AS School
FROM Patron
WHERE rtrim(ltrim(SchoolName)) <> ''
	AND SchoolName IS NOT NULL
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY rtrim(ltrim(SchoolName))
