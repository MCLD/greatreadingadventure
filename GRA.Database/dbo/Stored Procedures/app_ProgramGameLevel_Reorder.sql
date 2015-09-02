
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_Reorder] @PGID INT
AS
UPDATE ProgramGameLevel
SET LevelNumber = rowNumber
FROM ProgramGameLevel
INNER JOIN (
	SELECT PGLID,
		LevelNumber,
		row_number() OVER (
			ORDER BY LevelNumber ASC
			) AS rowNumber
	FROM ProgramGameLevel
	WHERE PGID = @PGID
	) drRowNumbers ON drRowNumbers.PGLID = ProgramGameLevel.PGLID
	AND PGID = @PGID
WHERE PGID = @PGID
