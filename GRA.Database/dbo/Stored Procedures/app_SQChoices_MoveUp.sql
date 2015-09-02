
CREATE PROCEDURE [dbo].[app_SQChoices_MoveUp] @SQCID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@QID INT

SELECT @CurrentRecordLocation = ChoiceOrder,
	@QID = QID
FROM SQChoices
WHERE SQCID = @SQCID

EXEC [dbo].[app_SQChoices_Reorder] @QID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = SQCID
	FROM SQChoices
	WHERE ChoiceOrder = (@CurrentRecordLocation - 1)
		AND QID = @QID

	UPDATE SQChoices
	SET ChoiceOrder = @CurrentRecordLocation - 1
	WHERE SQCID = @SQCID

	UPDATE SQChoices
	SET ChoiceOrder = @CurrentRecordLocation
	WHERE SQCID = @PreviousRecordID
END
