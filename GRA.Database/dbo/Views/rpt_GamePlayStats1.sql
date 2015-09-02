
CREATE VIEW [dbo].[rpt_GamePlayStats1]
AS
WITH stats
AS (
	SELECT DISTINCT gps.GPSID,
		gps.PID,
		p.Username,
		p.FirstName,
		p.LastName,
		p.Gender,
		p.EmailAddress,
		gps.MGID,
		g.GameName,
		g.AdminName,
		gps.MGType,
		g.MiniGameTypeName,
		gps.CompletedPlay,
		gps.Difficulty,
		gps.Started,
		gps.Completed
	FROM dbo.GamePlayStats AS gps
	LEFT JOIN dbo.Patron AS p ON gps.PID = p.PID
	LEFT JOIN dbo.Minigame AS g ON gps.MGID = g.MGID
	)
SELECT DISTINCT TOP (100) PERCENT PID,
	Username,
	FirstName,
	LastName,
	Gender,
	EmailAddress,
	MGID,
	GameName,
	AdminName,
	MGType,
	MiniGameTypeName,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Easy')
		) AS EasyLevelStated,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Easy')
			AND (CompletedPlay = 1)
		) AS EasyLevelCompleted,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Medium')
		) AS MediumLevelStated,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Medium')
			AND (CompletedPlay = 1)
		) AS MediumLevelCompleted,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Hard')
		) AS HardLevelStated,
	(
		SELECT COUNT(*) AS Expr1
		FROM stats AS s1
		WHERE (PID = s.PID)
			AND (MGID = s.MGID)
			AND (Difficulty = 'Hard')
			AND (CompletedPlay = 1)
		) AS HardLevelCompleted
FROM stats AS s
ORDER BY Username,
	FirstName,
	LastName,
	Gender,
	EmailAddress,
	MGID,
	GameName,
	AdminName,
	MGType,
	MiniGameTypeName
