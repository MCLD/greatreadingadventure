
CREATE PROCEDURE [dbo].[app_LibraryCrosswalk_GetAll] @TenID INT = NULL
AS
DECLARE @Libraries TABLE (
	CID INT NOT NULL,
	Code VARCHAR(50) NOT NULL
	)

INSERT INTO @Libraries
SELECT c.CID,
	c.Code
FROM Code c
INNER JOIN CodeType t ON c.CTID = t.CTID
WHERE t.CodeTypeName = 'Branch'
	AND (
		t.TenID = @TenID
		OR @TenID IS NULL
		)

DELETE
FROM [LibraryCrosswalk]
WHERE BranchID NOT IN (
		SELECT CID
		FROM @Libraries
		)
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)

SELECT isnull(w.ID, 0) AS ID,
	isnull(l.CID, 0) AS BranchID,
	isnull(w.DistrictID, 0) AS DistrictID,
	isnull(w.City, '') AS City
FROM [LibraryCrosswalk] w
RIGHT JOIN @Libraries l ON w.BranchID = l.CID
ORDER BY l.Code
