
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SurveyResults_Delete] @SRID INT
AS
DELETE
FROM dbo.SurveyAnswers
WHERE SRID = @SRID

DELETE
FROM [SurveyResults]
WHERE SRID = @SRID
