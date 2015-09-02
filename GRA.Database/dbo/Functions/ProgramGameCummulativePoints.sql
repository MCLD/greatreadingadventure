
CREATE FUNCTION [dbo].[ProgramGameCummulativePoints] (@PGID INT)
RETURNS @Levels TABLE (
	PGID INT,
	GameLevel INT,
	PointsNeeded INT,
	IsBonus BIT
	)
AS
BEGIN
	DECLARE @T TABLE (
		PGID INT,
		GameLevel INT,
		PointsNeeded INT,
		IsBonus BIT
		)

	INSERT INTO @T
	VALUES (
		0,
		0,
		0,
		0
		)

	INSERT INTO @T
	SELECT @PGID,
		LevelNumber,
		PointNumber,
		0
	FROM ProgramGame pg
	INNER JOIN ProgramGameLevel pgl ON pg.PGID = pgl.PGID
	WHERE pg.PGID = @PGID
	ORDER BY LevelNumber

	DECLARE @i INT,
		@max INT,
		@numLevels INT

	SELECT @i = 0,
		@max = 20

	SELECT @numLevels = COUNT(*)
	FROM ProgramGameLevel
	WHERE PGID = @PGID

	WHILE @i < @max
	BEGIN
		INSERT INTO @t
		SELECT @PGID,
			LevelNumber + (@i + 1) * @numLevels,
			PointNumber * BonusLevelPointMultiplier,
			1
		FROM ProgramGame pg
		INNER JOIN ProgramGameLevel pgl ON pg.PGID = pgl.PGID
		WHERE pg.PGID = @PGID
		ORDER BY LevelNumber

		SELECT @i = @i + 1
	END

	INSERT INTO @Levels
	SELECT @PGID,
		t1.GameLevel,
		+ isnull(sum(t2.PointsNeeded) + t1.PointsNeeded, 0),
		t1.IsBonus
	FROM @T t1
	LEFT JOIN @T t2 ON t1.GameLevel > t2.GameLevel
	GROUP BY t1.GameLevel,
		t1.PointsNeeded,
		t1.IsBonus

	DELETE
	FROM @Levels
	WHERE GameLevel = 0

	RETURN
END
