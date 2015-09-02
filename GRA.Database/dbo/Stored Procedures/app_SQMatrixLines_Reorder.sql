
CREATE PROCEDURE [dbo].[app_SQMatrixLines_Reorder] @QID INT
AS
UPDATE SQMatrixLines
SET LineOrder = rowNumber
FROM SQMatrixLines
INNER JOIN (
	SELECT SQMLID,
		LineOrder,
		row_number() OVER (
			ORDER BY LineOrder ASC
			) AS rowNumber
	FROM SQMatrixLines
	WHERE QID = @QID
	) drRowNumbers ON drRowNumbers.SQMLID = SQMatrixLines.SQMLID
	AND QID = @QID
WHERE QID = @QID
