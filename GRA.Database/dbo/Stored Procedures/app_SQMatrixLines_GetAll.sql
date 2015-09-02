
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SQMatrixLines_GetAll] @QID INT
AS
SELECT *,
	(
		SELECT isnull(Max(LineOrder), 0)
		FROM [SQMatrixLines]
		WHERE QID = @QID
		) AS MAX
FROM [SQMatrixLines]
WHERE QID = @QID
ORDER BY LineOrder
