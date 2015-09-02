
CREATE PROCEDURE [dbo].[app_SQMatrixLines_MoveUp] @SQMLID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@QID INT

SELECT @CurrentRecordLocation = LineOrder,
	@QID = QID
FROM SQMatrixLines
WHERE SQMLID = @SQMLID

EXEC [dbo].[app_SQMatrixLines_Reorder] @QID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = SQMLID
	FROM SQMatrixLines
	WHERE LineOrder = (@CurrentRecordLocation - 1)
		AND QID = @QID

	UPDATE SQMatrixLines
	SET LineOrder = @CurrentRecordLocation - 1
	WHERE SQMLID = @SQMLID

	UPDATE SQMatrixLines
	SET LineOrder = @CurrentRecordLocation
	WHERE SQMLID = @PreviousRecordID
END
