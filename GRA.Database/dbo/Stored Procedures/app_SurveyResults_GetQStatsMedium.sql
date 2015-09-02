
CREATE PROCEDURE [dbo].[app_SurveyResults_GetQStatsMedium] @SID INT = NULL,
	@QID INT = NULL,
	@SQMLID INT = 0,
	@SourceType VARCHAR(250) = NULL,
	@SourceID INT = NULL
AS
SELECT REPLACE(CONVERT(VARCHAR(8000), a.ChoiceAnswerText), '~|~', ' AND ') AS ChoiceText,
	CONVERT(VARCHAR(8000), a.ChoiceAnswerText) AS ChoiceTextORIGINAL,
	COUNT(*) AS Count
FROM SurveyAnswers a
INNER JOIN SurveyResults r ON r.SRID = a.SRID
	AND (
		r.Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		r.SourceID = @SourceID
		OR @SourceID IS NULL
		)
WHERE a.SID = @SID
	AND QID = @QID
	AND SQMLID = @SQMLID
GROUP BY CONVERT(VARCHAR(8000), ChoiceAnswerText)
