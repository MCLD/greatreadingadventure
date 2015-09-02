
/****** Object:  StoredProcedure [dbo].[app_PatronReadingLog_Insert]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronReadingLog_Insert] (
	@PID INT,
	@ReadingType INT,
	@ReadingTypeLabel VARCHAR(50),
	@ReadingAmount INT,
	@ReadingPoints INT,
	@LoggingDate VARCHAR(50),
	@Author VARCHAR(50),
	@Title VARCHAR(150),
	@HasReview BIT,
	@ReviewID INT,
	@PRLID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronReadingLog (
		PID,
		ReadingType,
		ReadingTypeLabel,
		ReadingAmount,
		ReadingPoints,
		LoggingDate,
		Author,
		Title,
		HasReview,
		ReviewID
		)
	VALUES (
		@PID,
		@ReadingType,
		@ReadingTypeLabel,
		@ReadingAmount,
		@ReadingPoints,
		@LoggingDate,
		@Author,
		@Title,
		@HasReview,
		@ReviewID
		)

	SELECT @PRLID = SCOPE_IDENTITY()
END
