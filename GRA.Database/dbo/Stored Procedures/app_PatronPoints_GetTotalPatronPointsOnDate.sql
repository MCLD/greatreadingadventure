
CREATE PROCEDURE [dbo].[app_PatronPoints_GetTotalPatronPointsOnDate] (
	@PID INT,
	@Date DATETIME = NULL
	)
AS
BEGIN
	IF @Date IS NULL
		SELECT @Date = GETDATE()

	IF (
			EXISTS (
				SELECT isnull(SUM(isnull(NumPoints, 0)), 0) AS TotalPoints
				FROM PatronPoints
				WHERE PID = @PID
				)
			)
		--And convert(varchar, AwardDate, 101) = convert(varchar, @Date, 101)
		SELECT isnull(SUM(isnull(NumPoints, 0)), 0) AS TotalPoints
		FROM PatronPoints
		WHERE PID = @PID
			AND convert(VARCHAR, AwardDate, 101) = convert(VARCHAR, @Date, 101)
	ELSE
		SELECT 0 AS TotalPoints
END
