
CREATE PROCEDURE [dbo].[app_SurveyQuestion_GetAll] @SID INT = NULL
AS
SELECT *,
	(
		SELECT isnull(Max(QNumber), 0)
		FROM SurveyQuestion
		WHERE SID = @SID
		) AS MAX
FROM [SurveyQuestion]
WHERE SID = @SID
ORDER BY QNumber
