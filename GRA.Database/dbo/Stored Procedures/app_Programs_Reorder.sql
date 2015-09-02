
CREATE PROCEDURE [dbo].[app_Programs_Reorder] @TenID INT
AS
UPDATE Programs
SET POrder = rowNumber
FROM Programs
INNER JOIN (
	SELECT PID,
		POrder,
		row_number() OVER (
			ORDER BY POrder ASC
			) AS rowNumber
	FROM Programs
	WHERE TenID = @TenID
	) drRowNumbers ON drRowNumbers.PID = Programs.PID
	AND TenID = @TenID
