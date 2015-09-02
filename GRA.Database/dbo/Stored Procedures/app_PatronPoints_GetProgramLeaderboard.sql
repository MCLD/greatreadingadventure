
CREATE PROCEDURE [dbo].[app_PatronPoints_GetProgramLeaderboard] @ProgId INT = 0,
	@TenID INT = NULL
AS
IF @TenID IS NULL
	SELECT @TenID = TenID
	FROM Programs
	WHERE PID = @ProgId

SELECT TOP 10 pp.PID,
	isnull(SUM(isnull(convert(BIGINT, NumPoints), 0)), 0) AS TotalPoints,
	p.Username,
	p.AvatarID
INTO #TempLB
FROM PatronPoints pp
INNER JOIN Patron p ON pp.PID = p.PID
	AND p.TenID = @TenID
WHERE p.ProgID = @ProgId
GROUP BY pp.PID,
	p.Username,
	p.AvatarID
ORDER BY TotalPoints DESC

UPDATE #TempLB
SET TotalPoints = 20000000
WHERE TotalPoints > 20000000

SELECT PID,
	Username,
	AvatarID,
	CONVERT(INT, TotalPoints) AS TotalPoints,
	ROW_NUMBER() OVER (
		ORDER BY TotalPoints DESC
		) AS Rank
FROM #TempLB
ORDER BY TotalPoints DESC
