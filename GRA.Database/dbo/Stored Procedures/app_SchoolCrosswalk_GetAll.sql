
CREATE PROCEDURE [dbo].[app_SchoolCrosswalk_GetAll] @TenID INT = NULL
AS
DECLARE @Schools TABLE (
	CID INT NOT NULL,
	Code VARCHAR(50) NOT NULL
	)

INSERT INTO @Schools
SELECT c.CID,
	c.Code
FROM Code c
INNER JOIN CodeType t ON c.CTID = t.CTID
WHERE t.CodeTypeName = 'School'
	AND (
		t.TenID = @TenID
		OR @TenID IS NULL
		)

DELETE
FROM [SchoolCrosswalk]
WHERE SchoolID NOT IN (
		SELECT CID
		FROM @Schools
		)
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)

SELECT isnull(w.ID, 0) AS ID,
	isnull(l.CID, 0) AS SchoolID,
	isnull(w.SchTypeID, 0) AS SchTypeID,
	isnull(w.DistrictID, 0) AS DistrictID,
	isnull(w.City, '') AS City,
	isnull(w.MinGrade, 0) AS MinGrade,
	isnull(w.MaxGrade, 0) AS MaxGrade,
	isnull(w.MinAge, 0) AS MinAge,
	isnull(w.MaxAge, 0) AS MaxAge,
	isnull(l.Code, '') AS SchoolName,
	RANK() OVER (
		ORDER BY l.Code ASC
		) AS RANK
FROM [SchoolCrosswalk] w
RIGHT JOIN @Schools l ON w.SchoolID = l.CID
ORDER BY l.Code
