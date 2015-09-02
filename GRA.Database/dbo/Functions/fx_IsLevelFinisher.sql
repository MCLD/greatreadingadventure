
CREATE FUNCTION [dbo].[fx_IsLevelFinisher] (
	@PID INT,
	@ProgID INT,
	@Level INT = NULL
	)
RETURNS BIT
AS
BEGIN
	DECLARE @ret BIT
	DECLARE @GameCompletionPoints INT
	DECLARE @UserPoints INT

	IF (
			@PID IS NULL
			OR @ProgID IS NULL
			OR @ProgID = 0
			)
	BEGIN
		SET @ret = 0
	END
	ELSE
	BEGIN
		SELECT @GameCompletionPoints = SUM(isnull(pgl.PointNumber, 0))
		FROM ProgramGame pg
		LEFT JOIN ProgramGameLevel pgl ON pg.PGID = pgl.PGID
		LEFT JOIN Programs p ON p.ProgramGameID = pg.PGID
		WHERE p.PID = @ProgID
			AND (
				pgl.LevelNumber <= @Level
				OR @Level IS NULL
				)

		SELECT @UserPoints = SUM(isnull(NumPoints, 0))
		FROM PatronPoints
		WHERE PID = @PID

		SELECT @ret = CASE 
				WHEN @GameCompletionPoints < @UserPoints
					THEN 0
				ELSE 1
				END
	END

	RETURN @ret
END
