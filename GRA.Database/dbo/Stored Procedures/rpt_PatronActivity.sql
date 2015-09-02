
CREATE PROCEDURE [dbo].[rpt_PatronActivity] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL
AS
SET ARITHABORT OFF;
SET ANSI_WARNINGS OFF;

SELECT pg.AdminName AS Program,
	p.Username,
	p.FirstName,
	p.LastName,
	ISNULL(c1.Code, '') AS LibraryDistrict,
	ISNULL(c2.Code, '') AS Library,
	ISNULL(c3.Code, '') AS SchoolDistrict,
	ISNULL(c4.Code, '') AS School,
	p.Age,
	p.SchoolGrade,
	CASE 
		WHEN Score1Date IS NOT NULL
			THEN CONVERT(VARCHAR(50), Score1)
		ELSE 'N/A'
		END AS Score1,
	CASE 
		WHEN Score1Date IS NOT NULL
			THEN CONVERT(VARCHAR(10), Score1Date, 101)
		ELSE 'N/A'
		END AS Score1Date,
	CASE 
		WHEN Score2Date IS NOT NULL
			THEN CONVERT(VARCHAR(50), Score2)
		ELSE 'N/A'
		END AS Score2,
	CASE 
		WHEN Score2Date IS NOT NULL
			THEN CONVERT(VARCHAR(10), Score2Date, 101)
		ELSE 'N/A'
		END AS Score2Date,
	CASE 
		WHEN Score1Date IS NOT NULL
			AND Score2Date IS NOT NULL
			THEN CONVERT(VARCHAR(50), Score2 - Score1)
		ELSE 'N/A'
		END AS ScoreDifference,
	isnull((
			SELECT SUM(NumPoints)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isReading = 1
			), 0) AS [# Points For Reading],
	isnull((
			SELECT SUM(NumPoints)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isEvent = 1
			), 0) AS [# Points For Events],
	isnull((
			SELECT SUM(NumPoints)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isGameLevelActivity = 1
			), 0) AS [# Points For Games],
	isnull((
			SELECT SUM(NumPoints)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isBookList = 1
			), 0) AS [# Points For Book Lists],
	isnull((
			SELECT COUNT(1)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isReading = 1
			), 0) AS [# Times Logged Reading],
	isnull((
			SELECT COUNT(1)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isEvent = 1
			), 0) AS [# Events Attended],
	isnull((
			SELECT COUNT(1)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isGameLevelActivity = 1
			), 0) AS [# Times Logged Games],
	isnull((
			SELECT COUNT(1)
			FROM PatronPoints pp
			WHERE pp.PID = p.PID
				AND isBookList = 1
			), 0) AS [# Book Lists Completed],
	isnull((
			SELECT COUNT(1)
			FROM PatronBadges pp
			WHERE pp.PID = p.PID
			), 0) AS [# Badges Earned],
	isnull((
			SELECT COUNT(1)
			FROM GamePlayStats pp
			WHERE pp.PID = p.PID
				AND CompletedPlay = 1
			), 0) AS [# Minigames Played]
FROM Patron p
LEFT JOIN Code c1 ON p.District = c1.CID
LEFT JOIN Code c2 ON p.PrimaryLibrary = c2.CID
LEFT JOIN Code c3 ON p.SDistrict = c3.CID
LEFT JOIN Code c4 ON p.SchoolName = c4.CID
LEFT JOIN Programs pg ON ProgID = pg.PID
	AND p.TenID = pg.TenID
WHERE p.TenID = @TenID
	AND Username IS NOT NULL
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
ORDER BY AdminName,
	p.Username,
	p.FirstName,
	p.LastName
