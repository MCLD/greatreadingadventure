
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_Survey_Delete] @SID INT
AS
DELETE
FROM SQChoices
WHERE QID IN (
		SELECT QID
		FROM SurveyQuestion
		WHERE SID = @SID
		)

DELETE
FROM SQMatrixLines
WHERE QID IN (
		SELECT QID
		FROM SurveyQuestion
		WHERE SID = @SID
		)

DELETE
FROM SurveyQuestion
WHERE SID = @SID

DELETE
FROM SurveyAnswers
WHERE SID = @SID

DELETE
FROM SurveyResults
WHERE SID = @SID

UPDATE Programs
SET PreTestID = 0
WHERE PreTestID = @SID

UPDATE Programs
SET PostTestID = 0
WHERE PostTestID = @SID

DELETE
FROM [Survey]
WHERE SID = @SID
