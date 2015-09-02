
CREATE PROCEDURE [dbo].[app_Patron_GetScoreRank] @PID INT,
	@WhichScore INT = 1
AS
IF @WhichScore = 1
BEGIN
	SELECT convert(DECIMAL(18, 2), (
				SELECT COUNT(*)
				FROM Patron p1
				WHERE p1.ProgID = p.ProgID
					AND p1.Score1 <= p.Score1
				) - 1) * 100.00 / convert(DECIMAL(18, 2), (
				SELECT COUNT(*)
				FROM Patron p1
				WHERE p1.ProgID = p.ProgID
				)) AS Percentile,
		(
			SELECT COUNT(*)
			FROM Patron p1
			WHERE p1.ProgID = p.ProgID
			) AS T,
		(
			SELECT COUNT(*)
			FROM Patron p1
			WHERE p1.ProgID = p.ProgID
				AND p1.Score1 <= p.Score1
			) - 1 AS R,
		Score1 AS Score
	FROM Patron p
	WHERE p.PID = @PID
END
ELSE
BEGIN
	SELECT convert(DECIMAL(18, 2), (
				SELECT COUNT(*)
				FROM Patron p1
				WHERE p1.ProgID = p.ProgID
					AND p1.Score2 <= p.Score2
				) - 1) * 100.00 / convert(DECIMAL(18, 2), (
				SELECT COUNT(*)
				FROM Patron p1
				WHERE p1.ProgID = p.ProgID
				)) AS Percentile,
		(
			SELECT COUNT(*)
			FROM Patron p1
			WHERE p1.ProgID = p.ProgID
			) AS T,
		(
			SELECT COUNT(*)
			FROM Patron p1
			WHERE p1.ProgID = p.ProgID
				AND p1.Score2 <= p.Score2
			) - 1 AS R,
		Score2 AS Score
	FROM Patron p
	WHERE p.PID = @PID
END
