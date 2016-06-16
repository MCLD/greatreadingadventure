CREATE PROCEDURE [dbo].[rpt_ProgramByBranch] @TenID INT,
	@EndDate DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF (@EndDate IS NULL)
	BEGIN
		SET @EndDate = GETDATE()
	END

	CREATE TABLE #BranchStats (
		[BranchId] INT,
		[ProgId] INT,
		[SignUps] INT,
		[Achievers] INT
		)

	INSERT INTO #BranchStats
	SELECT p.[PrimaryLibrary] AS [BranchId],
		p.[ProgId] AS [ProgId],
		COUNT(p.[PID]) AS [Signups],
		COUNT(pp.[pid]) AS [Achievers]
	FROM [Patron] p
	LEFT OUTER JOIN (
		SELECT pp.[PID]
		FROM [PatronPoints] pp
		INNER JOIN [Patron] p ON p.[PID] = pp.[PID]
			AND p.[TenID] = @TenID
		INNER JOIN [Programs] prg ON prg.[PID] = p.[ProgID]
			AND prg.[TenID] = @TenID
		WHERE p.[RegistrationDate] < @EndDate
			AND pp.[AwardDate] < @EndDate
		GROUP BY pp.[PID],
			prg.[CompletionPoints]
		HAVING SUM(pp.[NumPoints]) >= prg.[CompletionPoints]
		) pp ON p.[pid] = pp.[pid]
	WHERE p.[RegistrationDate] < @EndDate
	GROUP BY p.[PrimaryLibrary],
		p.[ProgID]

	SELECT p.[TabName] AS [Program],
		bs.Signups,
		bs.Achievers,
		p.[CompletionPoints] AS [Achiever Points]
	FROM [Programs] p
	INNER JOIN (
		SELECT ProgId,
			SUM(Signups) AS [Signups],
			SUM([Achievers]) AS [Achievers]
		FROM #BranchStats
		GROUP BY ProgId
		) AS bs ON bs.[ProgId] = p.[PID]

	DECLARE @ProgramId INT

	DECLARE PGM_CURSOR CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR
	SELECT DISTINCT [ProgId]
	FROM #BranchStats
	ORDER BY [ProgId]

	OPEN PGM_CURSOR

	FETCH NEXT
	FROM PGM_CURSOR
	INTO @ProgramId

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT libsys.[Description] AS [Library System],
			library.[Description] AS [Library],
			COALESCE(bs.[Signups], 0) AS [Signups],
			COALESCE(bs.[Achievers], 0) AS [Achievers]
		FROM [LibraryCrosswalk] lx
		INNER JOIN [Code] library ON lx.[BranchID] = library.[CID]
			AND library.[TenID] = @TenId
		INNER JOIN [Code] libsys ON lx.[DistrictID] = libsys.[CID]
			AND libsys.[TenID] = @TenId
		LEFT OUTER JOIN #BranchStats bs ON bs.[BranchId] = lx.[BranchId]
			AND bs.[ProgId] = @ProgramId
		ORDER BY lx.[DistrictId],
			lx.[BranchId]

		FETCH NEXT
		FROM PGM_CURSOR
		INTO @ProgramId
	END

	DROP TABLE #BranchStats
END