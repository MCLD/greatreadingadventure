
CREATE PROCEDURE [dbo].[rpt_ReadingActivityReport] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@ActivityType INT = 1,
	@TenID INT = NULL
AS
DECLARE @ActivityLabel VARCHAR(50)

SELECT @ActivityLabel = CASE @ActivityType
		WHEN 0
			THEN 'Books'
		WHEN 1
			THEN 'Pages'
		WHEN 2
			THEN 'Paragraphs'
		WHEN 3
			THEN 'Minutes'
		ELSE 'Unknown'
		END

SELECT isnull(pg.AdminName, 'N/A') AS Program,
	p.Username,
	isnull(p.FirstName, '') FirstName,
	isnull(p.LastName, '') LastName,
	isnull(convert(VARCHAR, Sum(dbo.fx_ConvertPoints(p.ProgID, isnull(l.ReadingPoints, 0), @ActivityType))), 'N/A') AS PatronActivityCount,
	@ActivityLabel AS Activity,
	p.PID AS PatronID
FROM Patron p
LEFT JOIN PatronReadingLog l ON p.PID = l.PID
LEFT JOIN Programs pg ON p.ProgID = pg.PID
WHERE p.TenID = @TenID
	AND p.ProgID > 0
	AND (
		p.ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		p.PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(p.SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(p.District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
GROUP BY p.PID,
	p.Username,
	p.FirstName,
	p.LastName,
	pg.AdminName,
	p.ProgID
ORDER BY pg.AdminName,
	p.FirstName,
	p.LastName
