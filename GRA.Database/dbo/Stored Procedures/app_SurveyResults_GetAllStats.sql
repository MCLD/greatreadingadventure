
CREATE PROCEDURE [dbo].[app_SurveyResults_GetAllStats] @TenID INT = 0,
	@SID INT = NULL,
	@SourceType VARCHAR(250) = NULL,
	@SourceID INT = NULL,
	@SchoolID INT = NULL
AS
SELECT isnull(COUNT(*), 0) AS NumTotal
FROM [SurveyResults]
INNER JOIN Patron ON SurveyResults.PID = Patron.PID
WHERE SurveyResults.TenID = @TenID
	AND (
		SID = @SID
		OR @SID IS NULL
		)
	AND (
		Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		SourceID = @SourceID
		OR @SourceID IS NULL
		)
	AND (
		SchoolName = @SchoolID
		OR @SchoolID IS NULL
		)

SELECT isnull(COUNT(DISTINCT SurveyResults.PID), 0) AS NumPatrons
FROM [SurveyResults]
INNER JOIN Patron ON SurveyResults.PID = Patron.PID
WHERE SurveyResults.TenID = @TenID
	AND (
		SID = @SID
		OR @SID IS NULL
		)
	AND (
		Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		SourceID = @SourceID
		OR @SourceID IS NULL
		)
	AND (
		SchoolName = @SchoolID
		OR @SchoolID IS NULL
		)

SELECT DISTINCT a.SID,
	a.QID,
	a.SQMLID,
	a.QType,
	q.QNumber --, q.QText
	,
	(
		CASE a.SQMLID
			WHEN (
					SELECT MIN(SQMLID)
					FROM SurveyAnswers b
					WHERE b.QID = a.QID
					)
				THEN 1
			ELSE 0
			END
		) ShowQText,
	(
		SELECT COUNT(*)
		FROM SurveyAnswers a2
		WHERE a.SID = a2.SID
			AND a.QID = a2.QID
			AND a.SQMLID = a2.SQMLID
		) AS NumAnswers
FROM SurveyAnswers a
INNER JOIN SurveyQuestion q ON a.QID = q.QID
INNER JOIN SurveyResults r ON r.SRID = a.SRID
	AND (
		r.Source = @SourceType
		OR @SourceType IS NULL
		)
	AND (
		r.SourceID = @SourceID
		OR @SourceID IS NULL
		)
INNER JOIN Patron p ON r.PID = p.PID
WHERE (
		a.SID = @SID
		OR @SID IS NULL
		)
	AND (
		p.SchoolName = @SchoolID
		OR @SchoolID IS NULL
		)
ORDER BY q.QNumber,
	a.SID,
	a.QID,
	a.SQMLID
