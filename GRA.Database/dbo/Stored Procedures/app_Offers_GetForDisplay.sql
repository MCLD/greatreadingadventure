
CREATE PROCEDURE [dbo].[app_Offers_GetForDisplay] @PID INT = 0,
	@TenID INT = NULL
AS
--declare @PID int
--select @PID = 100000
IF (@TenID IS NULL)
	SELECT @TenID = TenID
	FROM Patron
	WHERE PID = @PID

DECLARE @Zip VARCHAR(20)
DECLARE @Age INT,
	@ProgramId INT,
	@BranchId INT

SELECT @Age = isnull(Age, 0),
	@Zip = isnull(ZipCode, ''),
	@ProgramId = isnull(ProgID, 0),
	@BranchId = isnull(PrimaryLibrary, 0)
FROM Patron
WHERE PID = @PID

----------------------------------------------------------
--select @Age, @Zip, @Age-36, @ProgramId, @BranchId
--select  o.* 
--from Offer o
----------------------------------------------------------
SELECT *
INTO #temp
FROM Offer
WHERE TenID = @TenID
	AND Offer.isEnabled = 1
	AND (
		Offer.MaxImpressions = 0
		OR Offer.MaxImpressions > Offer.TotalImpressions
		)

DELETE
FROM #temp
WHERE AgeStart > 0
	AND AgeEnd = 0
	AND @Age < AgeStart

DELETE
FROM #temp
WHERE AgeEnd > 0
	AND AgeStart = 0
	AND @Age > AgeEnd

DELETE
FROM #temp
WHERE AgeEnd > 0
	AND AgeStart > 0
	AND (
		@Age < AgeStart
		OR @Age > AgeEnd
		)

DELETE
FROM #temp
WHERE ProgramId <> 0
	AND ProgramId <> @ProgramId

IF @BranchId <> 0
	DELETE
	FROM #temp
	WHERE BranchId <> 0
		AND BranchId <> @BranchId

IF @Zip <> ''
	DELETE
	FROM #temp
	WHERE ZipCode <> ''
		AND ZipCode <> left(@Zip, 5)

SELECT ROW_NUMBER() OVER (
		ORDER BY OID
		) AS Rank,
	*
FROM #temp
