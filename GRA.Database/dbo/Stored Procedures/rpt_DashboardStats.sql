
CREATE PROCEDURE [dbo].[rpt_DashboardStats] @ProgId INT = NULL,
	@BranchID INT = NULL,
	@School VARCHAR(50) = NULL,
	@LibSys VARCHAR(50) = NULL,
	@Level INT = NULL,
	@TenID INT = NULL
AS
SELECT AdminName AS Program,
	count(*) AS RegistrantCount
FROM Patron p
LEFT JOIN Programs pg ON p.ProgId = pg.PID
WHERE p.TenID = @TenID
	AND p.ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
GROUP BY AdminName

---------------------------------------------------------------------------------------------------------------
IF EXISTS (
		SELECT AdminName AS Program,
			isnull(count(*), 0) AS FinisherCount
		FROM Patron p
		RIGHT JOIN Programs pg ON p.ProgId = pg.PID
		WHERE p.TenID = @TenID
			AND p.ProgID > 0
			AND (
				ProgID = @ProgId
				OR @ProgId IS NULL
				)
			AND (
				PrimaryLibrary = @BranchID
				OR @BranchID IS NULL
				)
			AND (
				rtrim(ltrim(isnull(SchoolName, ''))) = @School
				OR @School IS NULL
				)
			AND (
				rtrim(ltrim(isnull(District, ''))) = @LibSys
				OR @LibSys IS NULL
				)
			AND [dbo].[fx_IsFinisher2](p.PID, pg.PID, @Level) = 1
		GROUP BY AdminName
		)
	SELECT AdminName AS Program,
		isnull(count(*), 0) AS FinisherCount
	FROM Patron p
	RIGHT JOIN Programs pg ON p.ProgId = pg.PID
	WHERE p.TenID = @TenID
		AND p.ProgID > 0
		AND (
			ProgID = @ProgId
			OR @ProgId IS NULL
			)
		AND (
			PrimaryLibrary = @BranchID
			OR @BranchID IS NULL
			)
		AND (
			rtrim(ltrim(isnull(SchoolName, ''))) = @School
			OR @School IS NULL
			)
		AND (
			rtrim(ltrim(isnull(District, ''))) = @LibSys
			OR @LibSys IS NULL
			)
		AND [dbo].[fx_IsFinisher2](p.PID, pg.PID, @Level) = 1
	GROUP BY AdminName
ELSE
	SELECT AdminName AS Program,
		0 AS FinisherCount
	FROM Programs pg
	WHERE pg.TenID = @TenID
		AND (
			PID = @ProgId
			OR @ProgId IS NULL
			)
	GROUP BY AdminName

---------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
DECLARE @current INT
DECLARE @ColCounter INT
DECLARE @SQL VARCHAR(7000)
DECLARE @SQL1 VARCHAR(7000)

-----------------------------------------------------------------------------------------
IF OBJECT_ID('tempdb..#Temp1') IS NOT NULL
	DROP TABLE #Temp1

SELECT AdminName AS Program,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END AS Age,
	count(*) AS RegistrantCount,
	- 1 AS IndexRank
INTO #temp1
FROM Patron p
LEFT JOIN Programs pg ON p.ProgId = pg.PID
WHERE p.TenID = @TenID
	AND p.ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
GROUP BY p.ProgId,
	AdminName,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END
ORDER BY AdminName,
	Age

UPDATE #Temp1
SET IndexRank = a.IndexRank
FROM (
	SELECT TOP 100 PERCENT Program,
		RANK() OVER (
			ORDER BY Program
			) AS IndexRank
	FROM #Temp1
	GROUP BY Program
	ORDER BY Program
	) a
INNER JOIN #Temp1 ON a.Program = #Temp1.Program

--select 1, * from #Temp1
/*
select @ColCounter = isnull(max(x.R) ,0)
from (
select RANK () over ( order by Count(Program)  desc ) as R
from #temp1
group by Program
) as x
*/
SELECT @ColCounter = isnull(max(IndexRank), 0)
FROM #temp1

--select @ColCounter
IF OBJECT_ID('tempdb..#StatsData') IS NOT NULL
	DROP TABLE #StatsData

CREATE TABLE #StatsData (Age INT)

IF OBJECT_ID('tempdb..#ProgramLabels') IS NOT NULL
	DROP TABLE #ProgramLabels

CREATE TABLE #ProgramLabels (Label VARCHAR(50))

INSERT INTO #ProgramLabels (Label)
SELECT Program
FROM #temp1
GROUP BY IndexRank,
	Program
ORDER BY IndexRank

--select * from #ProgramLabels
IF @ColCounter > 0
BEGIN
	SELECT @SQL = 'alter table #StatsData add '

	--SELECT @SQL1 = 'alter table #ProgramLabels add '
	SELECT @current = 1 --, @maxcounter = 8

	WHILE @current <= @ColCounter
	BEGIN
		SELECT @SQL = @SQL + 'PgmCount' + CONVERT(VARCHAR, @current) + ' int '

		SELECT @SQL1 = @SQL1 + 'PgmName' + CONVERT(VARCHAR, @current) + ' varchar(255)'

		IF @current < @ColCounter
		BEGIN
			SELECT @SQL = @SQL + ','

			SELECT @SQL1 = @SQL1 + ','
		END

		SELECT @current = @current + 1
	END

	PRINT @SQL

	--print @SQL1
	EXEC (@SQL)

	--EXEC (@SQL1)
	INSERT INTO #StatsData (Age)
	SELECT DISTINCT Age
	FROM #temp1
	ORDER BY Age

	SELECT @current = 1

	WHILE @current <= @ColCounter
	BEGIN
		SELECT @SQL = 'update #StatsData set ' + 'PgmCount' + CONVERT(VARCHAR, @current) + ' = a.RegistrantCount ' + 'from #Temp1 a inner join  #StatsData on a.Age = #StatsData.Age ' + ' and a.IndexRank = ' + CONVERT(VARCHAR, @current)

		SELECT @SQL1 = 'update #StatsData set ' + 'PgmCount' + CONVERT(VARCHAR, @current) + ' = 0 ' + 'where PgmCount' + CONVERT(VARCHAR, @current) + ' is null '

		EXEC (@SQL)

		EXEC (@SQL1)

		PRINT @SQL

		SELECT @current = @current + 1
	END
END

SELECT *
FROM #ProgramLabels

SELECT *
FROM #StatsData

-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
IF OBJECT_ID('tempdb..#Temp2') IS NOT NULL
	DROP TABLE #Temp2

SELECT AdminName AS Program,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END AS Age,
	count(*) AS FinisherCount,
	- 1 AS IndexRank
INTO #Temp2
FROM Patron p
LEFT JOIN Programs pg ON p.ProgId = pg.PID
WHERE p.TenID = @TenID
	AND p.ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
	AND [dbo].[fx_IsFinisher2](p.PID, pg.PID, @Level) = 1
GROUP BY p.ProgId,
	AdminName,
	CASE 
		WHEN DOB IS NOT NULL
			THEN FLOOR((CAST(GetDate() AS INT) - CAST(DOB AS INT)) / 365.25)
		ELSE CASE 
				WHEN Age IS NOT NULL
					THEN Age
				ELSE 0
				END
		END
ORDER BY AdminName,
	Age

UPDATE #Temp2
SET IndexRank = a.IndexRank
FROM (
	SELECT TOP 100 PERCENT Program,
		RANK() OVER (
			ORDER BY Program
			) AS IndexRank
	FROM #Temp2
	GROUP BY Program
	ORDER BY Program
	) a
INNER JOIN #Temp2 ON a.Program = #Temp2.Program

--select * from #Temp2
/*
select @ColCounter = isnull(max(x.R) ,0)
from (
select RANK () over ( order by Count(Program)  desc ) as R
from #Temp2
group by Program
) as x
*/
SELECT @ColCounter = isnull(max(IndexRank), 0)
FROM #Temp2

--select @ColCounter
IF OBJECT_ID('tempdb..#StatsData2') IS NOT NULL
	DROP TABLE #StatsData2

CREATE TABLE #StatsData2 (Age INT)

IF OBJECT_ID('tempdb..#ProgramLabels2') IS NOT NULL
	DROP TABLE #ProgramLabels2

CREATE TABLE #ProgramLabels2 (Label VARCHAR(50))

INSERT INTO #ProgramLabels2 (Label)
SELECT Program
FROM #Temp2
GROUP BY IndexRank,
	Program
ORDER BY IndexRank

--select * from #ProgramLabels2
IF @ColCounter > 0
BEGIN
	SELECT @SQL = 'alter table #StatsData2 add '

	--SELECT @SQL1 = 'alter table #ProgramLabels2 add '
	SELECT @current = 1 --, @maxcounter = 8

	WHILE @current <= @ColCounter
	BEGIN
		SELECT @SQL = @SQL + 'PgmCount' + CONVERT(VARCHAR, @current) + ' int '

		SELECT @SQL1 = @SQL1 + 'PgmName' + CONVERT(VARCHAR, @current) + ' varchar(255)'

		IF @current < @ColCounter
		BEGIN
			SELECT @SQL = @SQL + ','

			SELECT @SQL1 = @SQL1 + ','
		END

		SELECT @current = @current + 1
	END

	PRINT @SQL

	--print @SQL1
	EXEC (@SQL)

	--EXEC (@SQL1)
	INSERT INTO #StatsData2 (Age)
	SELECT DISTINCT Age
	FROM #Temp2
	ORDER BY Age

	SELECT @current = 1

	WHILE @current <= @ColCounter
	BEGIN
		SELECT @SQL = 'update #StatsData2 set ' + 'PgmCount' + CONVERT(VARCHAR, @current) + ' = a.FinisherCount ' + 'from #Temp2 a inner join  #StatsData2 on a.Age = #StatsData2.Age ' + ' and a.IndexRank = ' + CONVERT(VARCHAR, @current)

		SELECT @SQL1 = 'update #StatsData2 set ' + 'PgmCount' + CONVERT(VARCHAR, @current) + ' = 0 ' + 'where PgmCount' + CONVERT(VARCHAR, @current) + ' is null '

		EXEC (@SQL)

		EXEC (@SQL1)

		PRINT @SQL

		SELECT @current = @current + 1
	END
END

SELECT *
FROM #ProgramLabels2

SELECT *
FROM #StatsData2

-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------
SELECT pg.AdminName,
	Sum(CASE isnull(Gender, '')
			WHEN 'M'
				THEN 1
			ELSE 0
			END) AS MaleRegistrant,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 1
			ELSE 0
			END) AS FemaleRegistrant,
	Sum(CASE isnull(Gender, '')
			WHEN 'O'
				THEN 1
			ELSE 0
			END) AS OtherRegistrant,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 0
			WHEN 'O'
				THEN 0
			WHEN 'M'
				THEN 0
			ELSE 1
			END) AS NARegistrant,
	Sum(CASE isnull(Gender, '')
			WHEN 'M'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'O'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 0
			WHEN 'O'
				THEN 0
			WHEN 'M'
				THEN 0
			ELSE 1
			END) AS TotalRegistrant
FROM Patron
LEFT JOIN Programs pg ON ProgID = pg.PID
WHERE Patron.TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
GROUP BY AdminName
ORDER BY AdminName

---------------------------------------------------------------------------------------------------------------
SELECT pg.AdminName,
	Sum(CASE isnull(Gender, '')
			WHEN 'M'
				THEN 1
			ELSE 0
			END) AS MaleFinisher,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 1
			ELSE 0
			END) AS FemaleFinisher,
	Sum(CASE isnull(Gender, '')
			WHEN 'O'
				THEN 1
			ELSE 0
			END) AS OtherFinisher,
	Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 0
			WHEN 'O'
				THEN 0
			WHEN 'M'
				THEN 0
			ELSE 1
			END) AS NAFinisher,
	Sum(CASE isnull(Gender, '')
			WHEN 'M'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'O'
				THEN 1
			ELSE 0
			END) + Sum(CASE isnull(Gender, '')
			WHEN 'F'
				THEN 0
			WHEN 'O'
				THEN 0
			WHEN 'M'
				THEN 0
			ELSE 1
			END) AS TotalFinisher
FROM Patron
LEFT JOIN Programs pg ON ProgID = pg.PID
WHERE Patron.TenID = @TenID
	AND ProgID > 0
	AND (
		ProgID = @ProgId
		OR @ProgId IS NULL
		)
	AND (
		PrimaryLibrary = @BranchID
		OR @BranchID IS NULL
		)
	AND (
		rtrim(ltrim(isnull(SchoolName, ''))) = @School
		OR @School IS NULL
		)
	AND (
		rtrim(ltrim(isnull(District, ''))) = @LibSys
		OR @LibSys IS NULL
		)
	AND [dbo].[fx_IsFinisher2](Patron.PID, Pg.PID, @Level) = 1
GROUP BY AdminName
ORDER BY AdminName
