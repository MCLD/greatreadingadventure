
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyAnswers_GetAllExpanded] @SRID INT = NULL
AS
SELECT q.QText,
	a.*,
	(
		CASE a.SQMLID
			WHEN (
					SELECT MIN(SQMLID)
					FROM SurveyAnswers b
					WHERE b.SRID = @SRID
						AND b.QID = a.QID
					)
				THEN 1
			ELSE 0
			END
		) ShowQText
FROM SurveyAnswers a
INNER JOIN SurveyQuestion q ON a.QID = q.QID
WHERE SRID = @SRID
