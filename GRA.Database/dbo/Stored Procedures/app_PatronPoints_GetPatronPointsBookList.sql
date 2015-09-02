
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetPatronPointsBookList]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_PatronPoints_GetPatronPointsBookList] (
	@PID INT,
	@BLID INT = 0
	)
AS
BEGIN
	SELECT *
	FROM PatronPoints
	WHERE PID = @PID
		AND BookListID = @BLID
		AND AwardReasonCd = 2
END
