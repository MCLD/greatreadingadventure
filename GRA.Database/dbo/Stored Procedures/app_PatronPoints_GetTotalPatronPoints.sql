
/****** Object:  StoredProcedure [dbo].[app_PatronPoints_GetTotalPatronPoints]    Script Date: 01/05/2015 14:43:23 ******/
--Create the Insert Proc
CREATE PROCEDURE [dbo].[app_PatronPoints_GetTotalPatronPoints] (@PID INT)
AS
BEGIN
	IF (
			EXISTS (
				SELECT isnull(SUM(isnull(NumPoints, 0)), 0) AS TotalPoints
				FROM PatronPoints
				WHERE PID = @PID
				)
			)
		SELECT isnull(SUM(isnull(NumPoints, 0)), 0) AS TotalPoints
		FROM PatronPoints
		WHERE PID = @PID
	ELSE
		SELECT 0 AS TotalPoints
END
