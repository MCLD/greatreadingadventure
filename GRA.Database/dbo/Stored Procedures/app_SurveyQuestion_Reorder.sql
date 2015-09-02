
CREATE PROCEDURE [dbo].[app_SurveyQuestion_Reorder] @SID INT
AS
UPDATE SurveyQuestion
SET QNumber = rowNumber
FROM SurveyQuestion
INNER JOIN (
	SELECT QID,
		QNumber,
		row_number() OVER (
			ORDER BY QNumber ASC
			) AS rowNumber
	FROM SurveyQuestion
	WHERE SID = @SID
	) drRowNumbers ON drRowNumbers.QID = SurveyQuestion.QID
	AND SID = @SID
WHERE SID = @SID
