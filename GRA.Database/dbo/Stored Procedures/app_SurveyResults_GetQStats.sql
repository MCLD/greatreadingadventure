
CREATE PROCEDURE [dbo].[app_SurveyResults_GetQStats] @SID INT = NULL,
	@QID INT = NULL,
	@SQMLID INT = 0
AS
SELECT CONVERT(VARCHAR(8000), ChoiceAnswerText) AS ChoiceAnswerText,
	COUNT(*) AS Count
FROM SurveyAnswers a
WHERE SID = @SID
	AND QID = @QID
	AND SQMLID = @SQMLID
GROUP BY CONVERT(VARCHAR(8000), ChoiceAnswerText)
