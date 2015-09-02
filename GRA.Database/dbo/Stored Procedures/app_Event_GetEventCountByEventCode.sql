
CREATE PROCEDURE [dbo].[app_Event_GetEventCountByEventCode] (
	@EID INT = NULL,
	@key VARCHAR(50) = '',
	@TenID INT = NULL
	)
AS
BEGIN
	SELECT COUNT(*) AS NumCodes
	FROM Event
	WHERE SecretCode = @Key
		AND @key != ''
		AND (
			TenID = @TenID
			OR @TenID IS NULL
			)
		AND (
			EID != @EID
			OR @EID IS NULL
			)
END
