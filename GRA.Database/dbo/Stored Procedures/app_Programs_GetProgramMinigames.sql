
CREATE PROCEDURE [dbo].[app_Programs_GetProgramMinigames] @LevelIDs VARCHAR(1000) = '',
	@WhichMG INT = 0,
	@DefaultMG INT = 0
AS
IF @WhichMG = 0
	SELECT DISTINCT x.MGID,
		x.GameName
	FROM (
		SELECT mg.MGID,
			mg.GameName,
			- 1 AS LevelNumber
		FROM Minigame mg
		WHERE mg.MGID = @DefaultMG
		
		UNION
		
		SELECT mg.MGID,
			mg.GameName,
			pg.LevelNumber
		FROM Minigame mg
		INNER JOIN dbo.ProgramGameLevel pg ON mg.MGID = pg.Minigame1ID
		WHERE pg.PGLID IN (
				SELECT *
				FROM [dbo].[fnSplitBigInt](@LevelIDs)
				)
			--order by LevelNumber
		) AS x
ELSE
	SELECT DISTINCT x.MGID,
		x.GameName
	FROM (
		SELECT mg.MGID,
			mg.GameName,
			- 1 AS LevelNumber
		FROM Minigame mg
		WHERE mg.MGID = @DefaultMG
		
		UNION
		
		SELECT mg.MGID,
			mg.GameName,
			pg.LevelNumber
		FROM Minigame mg
		INNER JOIN dbo.ProgramGameLevel pg ON mg.MGID = pg.Minigame2ID
		WHERE pg.PGLID IN (
				SELECT *
				FROM [dbo].[fnSplitBigInt](@LevelIDs)
				)
			--order by LevelNumber
		) AS x
		/*
-- deprecated when added the default board game minigames
		select mg.* 
			from Minigame mg join dbo.ProgramGameLevel pg
				on mg.MGID = pg.Minigame2ID 
			where pg.PGLID in 
					(select * from [dbo].[fnSplitBigInt](@LevelIDs))
		order by pg.LevelNumber					
*/
