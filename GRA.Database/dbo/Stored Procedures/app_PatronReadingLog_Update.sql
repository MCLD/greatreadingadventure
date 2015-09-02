
/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_Update]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronReadingLog_Update] (
	@PRLID INT,
	@PID INT,
	@ReadingType INT,
	@ReadingTypeLabel VARCHAR(50),
	@ReadingAmount INT,
	@ReadingPoints INT,
	@LoggingDate VARCHAR(50),
	@Author VARCHAR(50),
	@Title VARCHAR(150),
	@HasReview BIT,
	@ReviewID INT
	)
AS
UPDATE PatronReadingLog
SET PID = @PID,
	ReadingType = @ReadingType,
	ReadingTypeLabel = @ReadingTypeLabel,
	ReadingAmount = @ReadingAmount,
	ReadingPoints = @ReadingPoints,
	LoggingDate = @LoggingDate,
	Author = @Author,
	Title = @Title,
	HasReview = @HasReview,
	ReviewID = @ReviewID
WHERE PRLID = @PRLID
