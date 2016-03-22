
CREATE PROCEDURE [dbo].[app_Event_GetEventsByEventCode] (
	@key VARCHAR(50) = '',
	@TenID INT = NULL
	)
AS
BEGIN
	SELECT *
	FROM Event
	WHERE CAST(GETDATE() AS DATE) >= CAST(EventDate AS DATE)
		AND SecretCode = @Key
		AND (
			TenID = @TenID
			OR @TenID IS NULL
			)
END
