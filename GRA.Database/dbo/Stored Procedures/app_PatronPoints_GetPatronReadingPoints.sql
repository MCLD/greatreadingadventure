
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetPatronReadingPoints]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronPoints_GetPatronReadingPoints] (@PID INT)
AS
BEGIN
	IF (
			EXISTS (
				SELECT isnull(SUM(isnull(NumPoints, 0)), 0) AS TotalPoints
				FROM PatronPoints
				WHERE PID = @PID AND AwardReasonCd = 0
				)
			)
		SELECT isnull(SUM(isnull(NumPoints, 0)), 0) AS TotalPoints
		FROM PatronPoints
		WHERE PID = @PID AND AwardReasonCd = 0
	ELSE
		SELECT 0 AS TotalPoints
END
