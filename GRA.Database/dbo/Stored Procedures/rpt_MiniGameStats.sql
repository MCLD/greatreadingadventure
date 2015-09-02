
CREATE PROCEDURE [dbo].[rpt_MiniGameStats] @start DATETIME = NULL,
	@end DATETIME = NULL,
	@MGID INT = NULL,
	@TenID INT = NULL
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
		Difficulty,
		Started,
		Completed
	FROM GamePlayStats gps
	INNER JOIN Patron p ON gps.PID = p.PID
		AND p.TenID = @TenID
	INNER JOIN Minigame g ON gps.MGID = g.MGID
	)
SELECT DISTINCT PID AS "Patron ID",
	Username,
	FirstName AS "First Name",
	LastName AS "Last Name",
	Gender,
	EmailAddress AS Email,
	MGID AS "MiniGame ID",
	GameName AS "Game Name",
	AdminName AS "Administrative Name",
	MGType AS "MiniGame Type ID",
	MiniGameTypeName AS "MiniGame Type",
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Easy'
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS EasyLevelStated,
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Easy'
			AND s1.CompletedPlay = 1
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS EasyLevelCompleted,
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Medium'
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS MediumLevelStated,
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Medium'
			AND s1.CompletedPlay = 1
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS MediumLevelCompleted,
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Hard'
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS HardLevelStated,
	(
		SELECT COUNT(*)
		FROM stats s1
		WHERE s1.PID = s.PID
			AND s1.mgid = s.mgid
			AND s1.Difficulty = 'Hard'
			AND s1.CompletedPlay = 1
			AND (
				@start IS NULL
				OR s1.Started >= @start
				)
			AND (
				@end IS NULL
				OR s1.Started <= @end
				)
		) AS HardLevelCompleted
FROM stats s
WHERE (
		@start IS NULL
		OR Started >= @start
		)
	AND (
		@end IS NULL
		OR Started >= @end
		)
	AND (
		@MGID IS NULL
		OR @MGID = MGID
		)
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
