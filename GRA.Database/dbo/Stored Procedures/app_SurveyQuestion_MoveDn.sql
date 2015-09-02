
CREATE PROCEDURE [dbo].[app_SurveyQuestion_MoveDn] @QID INT
AS
DECLARE @CurrentRecordLocation INT,
	@NextRecordID INT,
	@SID INT

SELECT @CurrentRecordLocation = QNumber,
	@SID = SID
FROM SurveyQuestion
WHERE QID = @QID

EXEC [dbo].[app_SurveyQuestion_Reorder] @SID

IF @CurrentRecordLocation < (
		SELECT MAX(QNumber)
		FROM SurveyQuestion
		WHERE SID = @SID
		)
BEGIN
	SELECT @NextRecordID = QID
	FROM SurveyQuestion
	WHERE QNumber = (@CurrentRecordLocation + 1)
		AND SID = @SID

	UPDATE SurveyQuestion
	SET QNumber = @CurrentRecordLocation + 1
	WHERE QID = @QID

	UPDATE SurveyQuestion
	SET QNumber = @CurrentRecordLocation
	WHERE QID = @NextRecordID
END
