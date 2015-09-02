
CREATE PROCEDURE [dbo].[app_SurveyResults_GetQStatsSimple] @SID INT = NULL,
	@QID INT = NULL,
	@SQMLID INT = 0,
	@SourceType VARCHAR(250) = NULL,
	@SourceID INT = NULL
AS
DECLARE @ChoiceAnswerText VARCHAR(8000)

DECLARE db_cursor CURSOR
FOR
SELECT a.ChoiceAnswerText
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

OPEN db_cursor

FETCH NEXT
FROM db_cursor
INTO @ChoiceAnswerText

CREATE TABLE #Stats (Value VARCHAR(256))

WHILE @@FETCH_STATUS = 0
BEGIN
	INSERT INTO #Stats
	SELECT Value
	FROM dbo.fnSplitString(REPLACE(@ChoiceAnswerText, '~|~', '|'), '|')

	FETCH NEXT
	FROM db_cursor
	INTO @ChoiceAnswerText
END

CLOSE db_cursor

DEALLOCATE db_cursor

SELECT Value,
	COUNT(*) AS Count
INTO #Stats2
FROM #Stats
GROUP BY Value

DROP TABLE #Stats

SELECT c.ChoiceText,
	ISNULL(d.Count, 0) AS Count
FROM SQChoices c
LEFT JOIN #Stats2 d ON c.ChoiceText = d.Value
WHERE QID = @QID
ORDER BY c.ChoiceOrder
