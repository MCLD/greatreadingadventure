
/****** Object:  StoredProcedure [dbo].[app_PatronReview_Insert]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronReview_Insert] (
	@PID INT,
	@PRLID INT,
	@Author VARCHAR(50),
	@Title VARCHAR(150),
	@Review TEXT,
	@isApproved BIT,
	@ReviewDate DATETIME,
	@ApprovalDate DATETIME,
	@ApprovedBy VARCHAR(50),
	@PRID INT OUTPUT
	)
AS
BEGIN
	INSERT INTO PatronReview (
		PID,
		PRLID,
		Author,
		Title,
		Review,
		isApproved,
		ReviewDate,
		ApprovalDate,
		ApprovedBy
		)
	VALUES (
		@PID,
		@PRLID,
		@Author,
		@Title,
		@Review,
		@isApproved,
		@ReviewDate,
		@ApprovalDate,
		@ApprovedBy
		)

	SELECT @PRID = SCOPE_IDENTITY()
END
