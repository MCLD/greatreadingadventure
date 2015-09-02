
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetPatronPointsByKeyword]    Script Date: 01/05/2015 14:43:23 ******/
CREATE PROCEDURE [dbo].[app_PatronPoints_GetPatronPointsByKeyword] (
	@PID INT,
	@Key VARCHAR(50) = ''
	)
AS
BEGIN
	SELECT *
	FROM PatronPoints
	WHERE PID = @PID
		AND EventCode = @Key
		AND AwardReasonCd = 1
END
