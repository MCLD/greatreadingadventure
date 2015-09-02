
CREATE PROCEDURE [dbo].[app_SurveyQuestion_MoveUp] @QID INT
AS
DECLARE @CurrentRecordLocation INT,
	@PreviousRecordID INT,
	@SID INT

SELECT @CurrentRecordLocation = QNumber,
	@SID = SID
FROM SurveyQuestion
WHERE QID = @QID

EXEC [dbo].[app_SurveyQuestion_Reorder] @SID

IF @CurrentRecordLocation > 1
BEGIN
	SELECT @PreviousRecordID = QID
	FROM SurveyQuestion
	WHERE QNumber = (@CurrentRecordLocation - 1)
		AND SID = @SID

	UPDATE SurveyQuestion
	SET QNumber = @CurrentRecordLocation - 1
	WHERE QID = @QID

	UPDATE SurveyQuestion
	SET QNumber = @CurrentRecordLocation
	WHERE QID = @PreviousRecordID
END
