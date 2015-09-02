
CREATE PROCEDURE [dbo].[app_SQMatrixLines_MoveDn] @SQMLID INT
AS
DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT,
	@QID INT

SELECT @CurrentRecordLocation = LineOrder,
	@QID = QID
FROM SQMatrixLines
WHERE SQMLID = @SQMLID

EXEC [dbo].[app_SQMatrixLines_Reorder] @QID

IF @CurrentRecordLocation < (
		SELECT MAX(LineOrder)
		FROM SQMatrixLines
		WHERE QID = @QID
		)
BEGIN
	SELECT @NextRecordID = SQMLID
	FROM SQMatrixLines
	WHERE LineOrder = (@CurrentRecordLocation + 1)
		AND QID = @QID

	UPDATE SQMatrixLines
	SET LineOrder = @CurrentRecordLocation + 1
	WHERE SQMLID = @SQMLID

	UPDATE SQMatrixLines
	SET LineOrder = @CurrentRecordLocation
	WHERE SQMLID = @NextRecordID
END
