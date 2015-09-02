
/****** Object:  StoredProcedure [dbo].[app_MGMatchingGame_GetRandomPlayItems]    Script Date: 01/05/2015 14:43:21 ******/
CREATE PROCEDURE [dbo].[app_MGMatchingGame_GetRandomPlayItems] @MAGID INT,
	@NumItems INT,
	@Difficulty INT
AS
DECLARE @SQL VARCHAR(8000)

CREATE TABLE #Temp1 (
	[ID] UNIQUEIDENTIFIER,
	[MAGTID] [int],
	[MAGID] [int],
	[MGID] [int],
	[Tile1UseMedium] [bit],
	[Tile1UseHard] [bit],
	[Tile2UseMedium] [bit],
	[Tile2UseHard] [bit],
	[Tile3UseMedium] [bit],
	[Tile3UseHard] [bit]
	)

CREATE TABLE #Temp2 (
	[MAGTID] [int],
	TileImage VARCHAR(255)
	)

SELECT @SQL = 'insert into #Temp1 
	select top ' + convert(VARCHAR, @NumItems) + ' NEWID() as ID, 
		[MAGTID], [MAGID], [MGID], [Tile1UseMedium], [Tile1UseHard], [Tile2UseMedium], [Tile2UseHard], [Tile3UseMedium],[Tile3UseHard]   from  dbo.MGMatchingGameTiles Where MAGID = ' + convert(VARCHAR, @MAGID) + '  order by id'

EXEC (@SQL)

--select * from #Temp1 
INSERT INTO #Temp2
SELECT MAGTID,
	('t1_' + CONVERT(VARCHAR, MAGTID) + '.png') AS TileImage
FROM #Temp1

INSERT INTO #Temp2
SELECT MAGTID,
	(
		CASE 
			WHEN @Difficulty = 1
				THEN 't1_' + CONVERT(VARCHAR, MAGTID) + '.png'
			WHEN @Difficulty = 2
				AND Tile1UseMedium = 1
				AND Tile2UseMedium = 0
				THEN 't1_' + CONVERT(VARCHAR, MAGTID) + '.png'
			WHEN @Difficulty = 2
				AND Tile2UseMedium = 1
				THEN 't2_' + CONVERT(VARCHAR, MAGTID) + '.png'
			WHEN @Difficulty = 3
				AND Tile2UseHard = 0
				AND Tile3UseHard = 0
				THEN 't1_' + CONVERT(VARCHAR, MAGTID) + '.png'
			WHEN @Difficulty = 3
				AND Tile2UseHard = 1
				AND Tile3UseHard = 0
				THEN 't2_' + CONVERT(VARCHAR, MAGTID) + '.png'
			WHEN @Difficulty = 3
				AND Tile3UseHard = 1
				THEN 't3_' + CONVERT(VARCHAR, MAGTID) + '.png'
			END
		) AS TileImage
FROM #Temp1

SELECT NEWID() AS ID,
	*
FROM #Temp2
ORDER BY ID

DROP TABLE #Temp1

DROP TABLE #Temp2
