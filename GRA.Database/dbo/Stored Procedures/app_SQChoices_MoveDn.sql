
CREATE PROCEDURE [dbo].[app_SQChoices_MoveDn] @SQCID INT
AS
DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT,
	@QID INT

SELECT @CurrentRecordLocation = ChoiceOrder,
	@QID = QID
FROM SQChoices
WHERE SQCID = @SQCID

EXEC [dbo].[app_SQChoices_Reorder] @QID

IF @CurrentRecordLocation < (
		SELECT MAX(ChoiceOrder)
		FROM SQChoices
		WHERE QID = @QID
		)
BEGIN
	SELECT @NextRecordID = SQCID
	FROM SQChoices
	WHERE ChoiceOrder = (@CurrentRecordLocation + 1)
		AND QID = @QID

	UPDATE SQChoices
	SET ChoiceOrder = @CurrentRecordLocation + 1
	WHERE SQCID = @SQCID

	UPDATE SQChoices
	SET ChoiceOrder = @CurrentRecordLocation
	WHERE SQCID = @NextRecordID
END
