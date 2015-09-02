
--Create the Select Proc
CREATE PROCEDURE [dbo].[app_SurveyAnswers_GetAll] @SRID INT
AS
SELECT a.*,
	q.QName,
	q.QText,
	q.QNumber
FROM [SurveyAnswers] a
INNER JOIN SurveyQuestion q ON a.QID = q.QID
WHERE a.SRID = @SRID
ORDER BY q.QNumber
