
CREATE PROCEDURE [dbo].[app_BookList_GetForDisplay] @PID INT = 0,
	@TenID INT = NULL,
	@SearchText NVARCHAR(255) = NULL
AS
-- variables for user information
DECLARE @ProgramId INT,
	@PatronTenant INT

-- load user information, Program and Tenant
SELECT @ProgramId = isnull(ProgID, 0),
	@PatronTenant = isnull([TenID], 0)
FROM Patron
WHERE PID = @PID

-- if no tenant was supplied, use the patron's tenant
IF (@TenID IS NULL)
BEGIN
	SET @TenID = @PatronTenant
END

-- temporary storage for unique BLIDs
CREATE TABLE #temp (BLID INT)

-- get BLIDs that are not associated with a program, associated with the user's program, and match the search text (if provided)
INSERT INTO #temp
SELECT DISTINCT bl.[BLID]
FROM BookList bl
LEFT OUTER JOIN [BookListBooks] blb ON blb.[BLID] = bl.[BLID]
WHERE (
		bl.[ProgID] = 0
		OR bl.[ProgID] = @ProgramId
		)
	AND (
		@SearchText IS NULL
		OR (
			bl.[ListName] LIKE @SearchText
			OR bl.[Description] LIKE @SearchText
			OR blb.[Title] LIKE @SearchText
			OR blb.[Author] LIKE @SearchText
			)
		)
	AND bl.[TenID] = @TenID

-- final organization of booklist
SELECT bl.*,
	(
		SELECT count(*)
		FROM [PatronBookLists] pbl
		WHERE pbl.[blid] = bl.[blid]
			AND pbl.[pid] = @pid
			AND pbl.[HasReadFlag] = 1
		) AS NumBooksCompleted
FROM #temp t
LEFT JOIN dbo.BookList bl ON bl.BLID = t.BLID
WHERE t.BLID IN (
		SELECT DISTINCT [BLID]
		FROM [BookListBooks]
		)
ORDER BY bl.[ListName]

DROP TABLE #temp
