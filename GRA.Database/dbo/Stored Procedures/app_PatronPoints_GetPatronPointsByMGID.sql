
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetPatronPointsByMGID]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_PatronPoints_GetPatronPointsByMGID] (
	@PID INT,
	@MGID INT = 0
	)
AS
BEGIN
	SELECT *
	FROM PatronPoints
	WHERE PID = @PID
		AND GameLevelActivityID = @MGID
		AND AwardReasonCd = 4
END
