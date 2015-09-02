
CREATE PROCEDURE [dbo].[app_Programs_GetDefaultProgramID] @TenID INT = NULL
AS
DECLARE @ID INT

SELECT TOP 1 @ID = PID
FROM [Programs]
WHERE IsActive = 1
	AND IsHidden = 0
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY POrder ASC

SELECT @ID

RETURN 0
