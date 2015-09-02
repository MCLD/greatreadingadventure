
/****** Object:  StoredProcedure [dbo].[app_PatronBookLists_Update]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Update Proc
CREATE PROCEDURE [dbo].[app_PatronBookLists_Update] (
	@PBLBID INT,
	@PID INT,
	@BLBID INT,
	@BLID INT,
	@HasReadFlag BIT,
	@LastModDate DATETIME
	)
AS
UPDATE PatronBookLists
SET PID = @PID,
	BLBID = @BLBID,
	BLID = @BLID,
	HasReadFlag = @HasReadFlag,
	LastModDate = @LastModDate
WHERE PBLBID = @PBLBID
