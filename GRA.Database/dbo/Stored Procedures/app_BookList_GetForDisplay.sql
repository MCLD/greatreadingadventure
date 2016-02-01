
CREATE PROCEDURE [dbo].[app_BookList_GetForDisplay] @PID INT = 0,
	@TenID INT = NULL
AS
--declare @PID int dbo.BookList
--select @PID = 100000
IF (@TenID IS NULL)
	SELECT @TenID = TenID
	FROM Patron
	WHERE PID = @PID

DECLARE @Lit1 INT
DECLARE @Lit2 INT,
	@ProgramId INT,
	@BranchId INT

SELECT @Lit1 = isnull(LiteracyLevel1, 0),
	@Lit2 = isnull(LiteracyLevel2, ''),
	@ProgramId = isnull(ProgID, 0),
	@BranchId = isnull(PrimaryLibrary, 0)
FROM Patron
WHERE PID = @PID

----------------------------------------------------------
--select @Age, @Zip, @Age-36, @ProgramId, @BranchId
--select  o.*
--from Offer o
----------------------------------------------------------
CREATE TABLE #temp (
	BLID INT,
	ListName VARCHAR(50),
	Description TEXT
	)

INSERT INTO #temp
SELECT BLID,
	ListName,
	Description
FROM BookList
WHERE LiteracyLevel1 > 0
	AND @Lit1 = LiteracyLevel1
	AND TenID = @TenID

INSERT INTO #temp
SELECT BLID,
	ListName,
	Description
FROM BookList
WHERE LiteracyLevel2 > 0
	AND @Lit2 = LiteracyLevel2
	AND TenID = @TenID

INSERT INTO #temp
SELECT BLID,
	ListName,
	Description
FROM BookList
WHERE ProgID > 0
	AND ProgID = @ProgramId
	AND TenID = @TenID

INSERT INTO #temp
SELECT BLID,
	ListName,
	Description
FROM BookList
WHERE LibraryID > 0
	AND LibraryID = @BranchId
	AND TenID = @TenID

INSERT INTO #temp
SELECT BLID,
	ListName,
	Description
FROM BookList
WHERE LibraryID = 0
	AND ProgID = 0
	AND LiteracyLevel1 = 0
	AND LiteracyLevel2 = 0
	AND TenID = @TenID

SELECT DISTINCT BLID
INTO #temp1
FROM #temp

DROP TABLE #temp

SELECT ROW_NUMBER() OVER (
		ORDER BY bl.BLID
		) AS Rank,
	bl.*,
	(select count(*) from [PatronBookLists] pbl WHERE pbl.[blid] = bl.[blid] AND pbl.[pid] = @pid AND pbl.[HasReadFlag] = 1) as NumBooksCompleted
FROM #temp1 t
LEFT JOIN dbo.BookList bl ON bl.BLID = t.BLID
