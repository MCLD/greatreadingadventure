
CREATE PROCEDURE [dbo].[app_Event_GetEventsByEventCode] (
	@startDate DATETIME,
	@endDate DATETIME,
	@key VARCHAR(50) = '',
	@TenID INT = NULL
	)
AS
BEGIN
	SELECT *
	FROM Event
	WHERE EventDate BETWEEN @startDate
			AND @endDate
		AND SecretCode = @Key
		AND (
			TenID = @TenID
			OR @TenID IS NULL
			)
END
