
CREATE PROCEDURE [dbo].[app_Programs_GetAllTabs] @TenID INT = NULL
AS
SELECT *,
	(
		SELECT COUNT(1)
		FROM Patron
		WHERE Patron.ProgID = Programs.PID
		) AS ParticipantCount,
	(
		SELECT isnull(Max(POrder), 1)
		FROM Programs
		) AS MAX
FROM [Programs]
WHERE IsActive = 1
	AND IsHidden = 0
	AND (
		TenID = @TenID
		OR @TenID IS NULL
		)
ORDER BY POrder ASC
