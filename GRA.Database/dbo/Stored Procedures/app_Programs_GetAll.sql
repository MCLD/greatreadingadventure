
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_Programs_GetAll] @TenID INT = NULL
AS
SELECT *,
	(
		SELECT COUNT(1)
		FROM Patron
		WHERE Patron.ProgID = Programs.PID
		) AS ParticipantCount
FROM [Programs]
WHERE (
		TenID = @TenID
		OR @TenID IS NULL
		)
