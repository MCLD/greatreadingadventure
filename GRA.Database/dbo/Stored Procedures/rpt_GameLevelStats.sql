
CREATE PROCEDURE [dbo].[rpt_GameLevelStats] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@TenID INT = NULL
AS
SET ARITHABORT OFF;
SET ANSI_WARNINGS OFF;

DECLARE @Levels TABLE (
	PGID INT,
	GameLevel INT,
	PointsNeeded INT,
	IsBonus BIT
	)

INSERT INTO @Levels
SELECT f.*
FROM ProgramGame pgm
CROSS APPLY dbo.ProgramGameCummulativePoints(pgm.PGID) f
WHERE pgm.TenID = @TenID

--select * from @Levels
SELECT ProgID,
	pg.AdminName,
	isnull((
			SELECT TOP 1 L.GameLevel
			FROM @Levels L
			WHERE L.PointsNeeded <= isNull((
						SELECT SUM(NumPoints)
						FROM PatronPoints pp
						WHERE pp.PID = p.PID
						), 0)
				AND L.PGID = pgm.PGID
			ORDER BY GameLevel DESC
			), 0) AS LevelAchieved
INTO #Temp
FROM Patron p
LEFT JOIN Programs pg ON ProgID = pg.PID
	AND p.TenID = pg.TenID
LEFT JOIN ProgramGame pgm ON pg.ProgramGameID = pgm.PGID
WHERE p.TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
--AND [dbo].[fx_IsFinisher](p.PID, Pg.PID) = 1
--group by p.PID, pgm.PGID, p.ProgID, pg.AdminName
ORDER BY p.PID,
	p.ProgID

SELECT AdminName,
	LevelAchieved,
	COUNT(LevelAchieved) AS FinisherCount
FROM #Temp
GROUP BY ProgID,
	AdminName,
	LevelAchieved
ORDER BY AdminName,
	LevelAchieved
