
--Create the Delete Proc
CREATE PROCEDURE [dbo].[app_SurveyAnswers_Delete] @SAID INT
AS
DELETE
FROM [SurveyAnswers]
WHERE SAID = @SAID
