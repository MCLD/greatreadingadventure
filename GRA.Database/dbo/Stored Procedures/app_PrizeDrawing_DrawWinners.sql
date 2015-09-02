
CREATE PROCEDURE [dbo].[app_PrizeDrawing_DrawWinners] @PDID INT = 0,
	@NumWinners INT = 1,
	@Additional INT = 0
AS
DECLARE @TID INT
DECLARE @TenID INT

SELECT @TID = TID,
	@TenID = TenID
FROM dbo.PrizeDrawing
WHERE PDID = @PDID

DECLARE @Gender VARCHAR(1),
	@SchoolName VARCHAR(50)
DECLARE @ProgID INT,
	@PrimaryLibrary INT
DECLARE @IncPrevWinnersFlag BIT
DECLARE @MinP INT,
	@MaxP INT,
	@MinR INT,
	@MaxR INT

SELECT @ProgID = ProgID,
	@PrimaryLibrary = PrimaryLibrary,
	@Gender = Gender,
	@SchoolName = SchoolName,
	@IncPrevWinnersFlag = IncPrevWinnersFlag,
	@MinP = MinPoints,
	@MaxP = MaxPoints,
	@MinR = MinReviews,
	@MaxR = MaxReviews
FROM dbo.PrizeTemplate
WHERE TID = @TID

SELECT PID,
	p.ProgID,
	p.PrimaryLibrary,
	p.SchoolName,
	p.Gender,
	isnull((
			SELECT SUM(NumPoints)
			FROM dbo.PatronPoints pp
			WHERE pp.PID = p.PID
				AND (
					AwardDate >= t.LogDateStart
					OR t.LogDateStart IS NULL
					)
				AND (
					AwardDate <= t.LogDateEnd
					OR t.LogDateEnd IS NULL
					)
			), 0) AS PatronPoints,
	isnull((
			SELECT count(PRID)
			FROM dbo.PatronReview pr
			WHERE pr.PID = p.PID
				AND (
					ReviewDate >= t.ReviewDateStart
					OR t.ReviewDateStart IS NULL
					)
				AND (
					ReviewDate <= t.ReviewDateEnd
					OR t.ReviewDateEnd IS NULL
					)
			), 0) AS PatronReviews,
	NEWID() AS Random
INTO #TEMP
FROM Patron p
INNER JOIN dbo.PrizeTemplate t ON t.TID = @TID
WHERE p.TenID = @TenID

IF (@ProgID <> 0)
	DELETE
	FROM #TEMP
	WHERE ProgID <> @ProgID

IF (@PrimaryLibrary <> 0)
	DELETE
	FROM #TEMP
	WHERE PrimaryLibrary <> @PrimaryLibrary

IF (@Gender <> '')
	DELETE
	FROM #TEMP
	WHERE Gender <> @Gender

IF (@SchoolName <> '')
	DELETE
	FROM #TEMP
	WHERE SchoolName <> @SchoolName

IF (@MinP <> 0)
	DELETE
	FROM #TEMP
	WHERE PatronPoints < @MinP

IF (@MaxP <> 0)
	DELETE
	FROM #TEMP
	WHERE PatronPoints > @MaxP

IF (@MinR <> 0)
	DELETE
	FROM #TEMP
	WHERE PatronReviews < @MinR

IF (@MaxR <> 0)
	DELETE
	FROM #TEMP
	WHERE PatronReviews > @MaxR

IF (@IncPrevWinnersFlag = 0)
	DELETE
	FROM #TEMP
	WHERE PID IN (
			SELECT DISTINCT PatronID
			FROM dbo.PrizeDrawingWinners
			)

IF (@Additional = 1)
	DELETE
	FROM #TEMP
	WHERE PID IN (
			SELECT DISTINCT PatronID
			FROM dbo.PrizeDrawingWinners
			WHERE PDID = @PDID
			)

INSERT INTO PrizeDrawingWinners (
	PDID,
	PatronID,
	NotificationID,
	PrizePickedUpFlag,
	LastModDate,
	LastModUser,
	AddedDate,
	AddedUser
	)
SELECT TOP (@NumWinners) @PDID,
	PID,
	0,
	0,
	GETDATE(),
	'N/A',
	GETDATE(),
	'N/A'
FROM #TEMP
ORDER BY Random

SELECT *
FROM PrizeDrawingWinners
WHERE PDID = @PDID
	AND NotificationID = 0
