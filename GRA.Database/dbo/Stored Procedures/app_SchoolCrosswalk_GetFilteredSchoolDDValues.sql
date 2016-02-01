
CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_GetFilteredSchoolDDValues] @SchTypeID INT = 0,
	@DistrictID INT = 0,
	@City VARCHAR(50) = '',
	@Grade INT = 0,
	@Age INT = 0,
	@TenID INT = NULL
AS
SELECT DISTINCT SchoolID AS CID,
	c.Code AS Code, c.[Description] as [Description]
FROM SchoolCrosswalk w
INNER JOIN Code c ON w.SchoolID = c.CID
WHERE (
		SchTypeID = @SchTypeID
		OR @SchTypeID IS NULL
		OR @SchTypeID = 0
		)
	AND (
		DistrictID = @DistrictID
		OR @DistrictID IS NULL
		OR @DistrictID = 0
		)
	AND (
		City = @City
		OR @City IS NULL
		OR @City = ''
		)
	AND (
		(
			MinGrade <= @Grade
			AND MaxGrade >= @Grade
			)
		OR @Grade IS NULL
		OR @Grade = 0
		OR (
			MinGrade = 0
			AND MaxGrade = 0
			)
		)
	AND (
		(
			MinAge <= @Age
			AND MaxAge >= @Age
			)
		OR @Age IS NULL
		OR @Age = 0
		OR (
			MinAge = 0
			AND MaxAge = 0
			)
		)
	AND (
		w.TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY [Description]
