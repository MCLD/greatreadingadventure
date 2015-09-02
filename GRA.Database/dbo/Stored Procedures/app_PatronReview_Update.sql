
/****** Object:  StoredProcedure [dbo].[app_PatronReview_Update]    Script Date: 01/05/2015 14:43:24 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronReview_Update] (
	@PRID INT,
	@PID INT,
	@PRLID INT,
	@Author VARCHAR(50),
	@Title VARCHAR(150),
	@Review TEXT,
	@isApproved BIT,
	@ReviewDate DATETIME,
	@ApprovalDate DATETIME,
	@ApprovedBy VARCHAR(50)
	)
AS
UPDATE PatronReview
SET PID = @PID,
	PRLID = @PRLID,
	Author = @Author,
	Title = @Title,
	Review = @Review,
	isApproved = @isApproved,
	ReviewDate = @ReviewDate,
	ApprovalDate = @ApprovalDate,
	ApprovedBy = @ApprovedBy
WHERE PRID = @PRID
