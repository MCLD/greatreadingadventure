
/****** Object:  StoredProcedure [dbo].[app_ProgramGameLevel_GetAll]    Script Date: 01/05/2015 14:43:25 ******/
CREATE PROCEDURE [dbo].[app_ProgramGameLevel_GetAll] @PGID INT = 0
AS
SELECT *,
	(
		SELECT isnull(Max(LevelNumber), 1)
		FROM [ProgramGameLevel]
		WHERE PGID = @PGID
		) AS MAX
FROM [ProgramGameLevel]
WHERE PGID = @PGID
ORDER BY LevelNumber
